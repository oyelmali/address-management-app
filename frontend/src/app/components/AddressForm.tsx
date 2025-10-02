'use client';

import { useForm } from 'react-hook-form';
import { X } from 'lucide-react';
import type { Address, CreateAddressDto, UpdateAddressDto } from '../types/address';

interface AddressFormProps {
  address?: Address;
  onSubmit: (data: CreateAddressDto | UpdateAddressDto) => Promise<void>;
  onClose: () => void;
}

export default function AddressForm({ address, onSubmit, onClose }: AddressFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<CreateAddressDto>({
    defaultValues: address || {
      region: '',
      city: '',
      branchNumber: '',
      branchType: '',
      street: '',
      phone: '',
      workingHoursWeekdays: '',
      workingHoursWeekend: '',
    },
  });

  const onSubmitForm = async (data: CreateAddressDto) => {
    await onSubmit(data);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
          <h2 className="text-xl font-semibold text-gray-800">
            {address ? 'Редагувати адресу' : 'Додати нову адресу'}
          </h2>
          <button
            onClick={onClose}
            className="p-1 hover:bg-gray-100 rounded transition-colors"
          >
            <X className="w-6 h-6 text-gray-500" />
          </button>
        </div>

        <form onSubmit={handleSubmit(onSubmitForm)} className="p-6 space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Область *
              </label>
              <input
                {...register('region', { required: "Поле обов'язкове" })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500"
                placeholder="Київська"
              />
              {errors.region && (
                <p className="mt-1 text-sm text-red-600">{errors.region.message}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Місто *
              </label>
              <input
                {...register('city', { required: "Поле обов'язкове" })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500"
                placeholder="Київ"
              />
              {errors.city && (
                <p className="mt-1 text-sm text-red-600">{errors.city.message}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Номер відділення *
              </label>
              <input
                {...register('branchNumber', { required: "Поле обов'язкове" })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500"
                placeholder="1"
              />
              {errors.branchNumber && (
                <p className="mt-1 text-sm text-red-600">{errors.branchNumber.message}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Тип відділення
              </label>
              <select
                {...register('branchType')}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500"
              >
                <option value="Відділення">Відділення</option>
                <option value="Поштомат InPost 24/7">Поштомат InPost 24/7</option>
              </select>
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Адреса *
            </label>
            <input
              {...register('street', { required: "Поле обов'язкове" })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500"
              placeholder="вул. Хрещатик, 1"
            />
            {errors.street && (
              <p className="mt-1 text-sm text-red-600">{errors.street.message}</p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Телефон *
            </label>
            <input
              {...register('phone', {
                required: "Поле обов'язкове",
                pattern: {
                  value: /^\+380\d{9}$/,
                  message: 'Формат: +380XXXXXXXXX',
                },
              })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500"
              placeholder="+380501234567"
            />
            {errors.phone && (
              <p className="mt-1 text-sm text-red-600">{errors.phone.message}</p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Графік роботи (будні)
            </label>
            <input
              {...register('workingHoursWeekdays')}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500"
              placeholder="пн-пт 08:00-20:00"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Графік роботи (вихідні)
            </label>
            <input
              {...register('workingHoursWeekend')}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500"
              placeholder="сб-нд 10:00-18:00"
            />
          </div>

          <div className="flex gap-3 pt-4">
            <button
              type="submit"
              disabled={isSubmitting}
              className="flex-1 bg-primary-600 text-white py-2 px-4 rounded-lg hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              {isSubmitting ? 'Збереження...' : 'Зберегти'}
            </button>
            <button
              type="button"
              onClick={onClose}
              className="flex-1 bg-gray-200 text-gray-700 py-2 px-4 rounded-lg hover:bg-gray-300 transition-colors"
            >
              Скасувати
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}