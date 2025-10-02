'use client';

import { Pencil, Trash2 } from 'lucide-react';
import type { Address } from '../types/address';
import { formatBranchName } from '../lib/utils';

interface AddressListProps {
  addresses: Address[];
  onEdit: (address: Address) => void;
  onDelete: (id: number) => void;
}

export default function AddressList({ addresses, onEdit, onDelete }: AddressListProps) {
  if (addresses.length === 0) {
    return (
      <div className="hidden md:block text-center py-12 text-gray-500">
        Адреси не знайдено
      </div>
    );
  }

  return (
    <div className="hidden md:block overflow-x-auto">
      <table className="w-full border-collapse">
        <thead>
          <tr className="bg-gray-50 border-b border-gray-200">
            <th className="px-4 py-3 text-left text-sm font-semibold text-gray-700">
              Відділення / Поштомат
            </th>
            <th className="px-4 py-3 text-left text-sm font-semibold text-gray-700">
              Адреса
            </th>
            <th className="px-4 py-3 text-left text-sm font-semibold text-gray-700">
              Графік роботи
            </th>
            <th className="px-4 py-3 text-left text-sm font-semibold text-gray-700">
              Телефон
            </th>
            <th className="px-4 py-3 text-left text-sm font-semibold text-gray-700">
              Дії
            </th>
          </tr>
        </thead>
        <tbody>
          {addresses.map((address, index) => (
            <tr
              key={address.id}
              className={`border-b border-gray-200 hover:bg-gray-50 transition-colors ${
                index % 2 === 0 ? 'bg-white' : 'bg-gray-50/50'
              }`}
            >
              <td className="px-4 py-3">
                <div className="font-medium text-primary-600">
                  {formatBranchName(address)}
                </div>
                <div className="text-sm text-gray-500">
                  {address.city}, {address.region.replace(' обл.', '')}
                </div>
              </td>
              <td className="px-4 py-3 text-sm text-gray-700">
                {address.street}
              </td>
              <td className="px-4 py-3 text-sm text-gray-600">
                <div>{address.workingHoursWeekdays}</div>
                <div className="text-gray-500">{address.workingHoursWeekend}</div>
              </td>
              <td className="px-4 py-3 text-sm text-gray-700">
                {address.phone}
              </td>
              <td className="px-4 py-3">
                <div className="flex gap-2">
                  <button
                    onClick={() => onEdit(address)}
                    className="p-1.5 text-primary-600 hover:bg-primary-50 rounded transition-colors"
                    title="Редагувати"
                  >
                    <Pencil className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => onDelete(address.id)}
                    className="p-1.5 text-red-600 hover:bg-red-50 rounded transition-colors"
                    title="Видалити"
                  >
                    <Trash2 className="w-4 h-4" />
                  </button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}