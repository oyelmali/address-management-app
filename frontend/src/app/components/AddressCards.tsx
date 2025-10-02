'use client';

import { Pencil, Trash2, Phone, Clock } from 'lucide-react';
import type { Address } from '../types/address';
import { formatBranchName } from '../lib/utils';

interface AddressCardsProps {
  addresses: Address[];
  onEdit: (address: Address) => void;
  onDelete: (id: number) => void;
}

export default function AddressCards({ addresses, onEdit, onDelete }: AddressCardsProps) {
  if (addresses.length === 0) {
    return (
      <div className="md:hidden text-center py-12 text-gray-500">
        Адреси не знайдено
      </div>
    );
  }

  return (
    <div className="md:hidden space-y-4">
      {addresses.map((address) => (
        <div
          key={address.id}
          className="bg-white border border-gray-200 rounded-lg p-4 shadow-sm"
        >
          <div className="flex justify-between items-start mb-3">
            <div>
              <h3 className="font-semibold text-primary-600 text-lg">
                {formatBranchName(address)}
              </h3>
              <p className="text-sm text-gray-500">
                {address.city}, {address.region.replace(' обл.', '')}
              </p>
            </div>
            <div className="flex gap-1">
              <button
                onClick={() => onEdit(address)}
                className="p-2 text-primary-600 hover:bg-primary-50 rounded transition-colors"
              >
                <Pencil className="w-4 h-4" />
              </button>
              <button
                onClick={() => onDelete(address.id)}
                className="p-2 text-red-600 hover:bg-red-50 rounded transition-colors"
              >
                <Trash2 className="w-4 h-4" />
              </button>
            </div>
          </div>

          <div className="space-y-2 text-sm">
            <p className="text-gray-700">{address.street}</p>

            <div className="flex items-center gap-2 text-gray-600">
              <Phone className="w-4 h-4" />
              <a href={`tel:${address.phone}`} className="hover:text-primary-600">
                {address.phone}
              </a>
            </div>

            <div className="flex items-start gap-2 text-gray-600">
              <Clock className="w-4 h-4 mt-0.5 flex-shrink-0" />
              <div>
                <div>{address.workingHoursWeekdays}</div>
                <div className="text-gray-500">{address.workingHoursWeekend}</div>
              </div>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}