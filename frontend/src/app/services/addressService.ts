import axios from 'axios';
import type { Address, CreateAddressDto, UpdateAddressDto, BulkImportResult } from '../types/address';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const addressService = {
  async getAll(): Promise<Address[]> {
    const response = await apiClient.get<Address[]>('/addresses');
    return response.data;
  },

  async getById(id: number): Promise<Address> {
    const response = await apiClient.get<Address>(`/addresses/${id}`);
    return response.data;
  },

  async create(data: CreateAddressDto): Promise<Address> {
    const response = await apiClient.post<Address>('/addresses', data);
    return response.data;
  },

  async update(id: number, data: UpdateAddressDto): Promise<void> {
    await apiClient.put(`/addresses/${id}`, data);
  },

  async delete(id: number): Promise<void> {
    await apiClient.delete(`/addresses/${id}`);
  },

  async search(term: string): Promise<Address[]> {
    const response = await apiClient.get<Address[]>(`/addresses/search?term=${encodeURIComponent(term)}`);
    return response.data;
  },

  async bulkImport(textAddresses: string[]): Promise<BulkImportResult> {
    const response = await apiClient.post<BulkImportResult>('/addresses/bulk-import', textAddresses);
    return response.data;
  },
};