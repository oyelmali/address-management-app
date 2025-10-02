'use client';

import { useState } from 'react';
import { X, Upload, AlertCircle, CheckCircle } from 'lucide-react';
import type { BulkImportResult } from '../types/address';

interface BulkImportModalProps {
  onImport: (texts: string[]) => Promise<BulkImportResult>;
  onClose: () => void;
}

export default function BulkImportModal({ onImport, onClose }: BulkImportModalProps) {
  const [text, setText] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [result, setResult] = useState<BulkImportResult | null>(null);

  const sampleData = `Запорізька обл., Запоріжжя, Відділення №16 (до 30 кг на одне місце): просп. Леніна, 84, +380974805040, пн-ср 08:00-15:00, сб-нд 10:00-12:00
Київська обл., Київ, Поштомат InPost 24/7, №2029: пр-т Повітрофлотський, 56а (цілодобовий поштомат біля маг."Billa"), +380974803040, пн-ср 08:00-15:00, сб-нд 10:00-20:00`;

  const handleImport = async () => {
    if (!text.trim()) return;

    setIsSubmitting(true);
    try {
      const lines = text
        .split('\n')
        .map((line) => line.trim())
        .filter((line) => line.length > 0);

      const importResult = await onImport(lines);
      setResult(importResult);
      
      // Auto close after 3 seconds if all successful
      if (importResult.failureCount === 0) {
        setTimeout(() => {
          handleClose();
        }, 2000);
      }
    } catch (error) {
      console.error('Import error:', error);
      setResult({
        successCount: 0,
        failureCount: 0,
        errors: ['Помилка при імпорті. Спробуйте ще раз.'],
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    setText('');
    setResult(null);
    onClose();
  };

  const loadSampleData = () => {
    setText(sampleData);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg shadow-xl max-w-3xl w-full max-h-[90vh] overflow-y-auto">
        <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
          <h2 className="text-xl font-semibold text-gray-800">
            Масовий імпорт адрес
          </h2>
          <button
            onClick={handleClose}
            className="p-1 hover:bg-gray-100 rounded transition-colors"
          >
            <X className="w-6 h-6 text-gray-500" />
          </button>
        </div>

        <div className="p-6 space-y-4">
          {!result ? (
            <>
              <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                <p className="text-sm text-blue-800">
                  <strong>Інструкція:</strong> Вставте адреси, кожна на окремому рядку.
                  Формат: <br />
                  <code className="text-xs bg-blue-100 px-2 py-1 rounded mt-2 inline-block">
                    Область обл., Місто, Тип №Номер: Адреса, Телефон, Графік
                  </code>
                </p>
                <button
                  onClick={loadSampleData}
                  className="mt-3 text-sm text-primary-600 hover:text-primary-700 font-medium"
                >
                  📋 Завантажити приклад
                </button>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Вставте адреси (по одній на рядок)
                </label>
                <textarea
                  value={text}
                  onChange={(e) => setText(e.target.value)}
                  rows={12}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500 font-mono text-sm"
                  placeholder="Запорізька обл., Запоріжжя, Відділення №16..."
                />
              </div>

              <div className="flex gap-3">
                <button
                  onClick={handleImport}
                  disabled={!text.trim() || isSubmitting}
                  className="flex-1 bg-primary-600 text-white py-2 px-4 rounded-lg hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center justify-center gap-2"
                >
                  <Upload className="w-5 h-5" />
                  {isSubmitting ? 'Імпорт...' : 'Імпортувати'}
                </button>
                <button
                  onClick={handleClose}
                  className="px-6 bg-gray-200 text-gray-700 py-2 rounded-lg hover:bg-gray-300 transition-colors"
                >
                  Скасувати
                </button>
              </div>
            </>
          ) : (
            <div className="space-y-4">
              <div className="flex items-center gap-3 p-4 bg-green-50 border border-green-200 rounded-lg">
                <CheckCircle className="w-6 h-6 text-green-600 flex-shrink-0" />
                <div>
                  <p className="font-semibold text-green-800">
                    Успішно імпортовано: {result.successCount}
                  </p>
                  {result.failureCount > 0 && (
                    <p className="text-sm text-red-600">
                      Помилок: {result.failureCount}
                    </p>
                  )}
                </div>
              </div>

              {result.errors.length > 0 && (
                <div className="space-y-2">
                  <div className="flex items-center gap-2 text-red-700 font-medium">
                    <AlertCircle className="w-5 h-5" />
                    <span>Помилки:</span>
                  </div>
                  <div className="bg-red-50 border border-red-200 rounded-lg p-4 max-h-48 overflow-y-auto">
                    <ul className="text-sm text-red-700 space-y-1 list-disc list-inside">
                      {result.errors.map((error, index) => (
                        <li key={index}>{error}</li>
                      ))}
                    </ul>
                  </div>
                </div>
              )}

              <button
                onClick={handleClose}
                className="w-full bg-primary-600 text-white py-2 px-4 rounded-lg hover:bg-primary-700 transition-colors"
              >
                Закрити
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}