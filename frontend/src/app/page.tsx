'use client';

import { useState, useEffect, useCallback, useRef } from 'react';
import { Plus, Upload } from 'lucide-react';
import type { Address, CreateAddressDto, UpdateAddressDto } from './types/address';
import { addressService } from './services/addressService';
import SearchBar from './components/SearchBar';
import AddressList from './components/AddressList';
import AddressCards from './components/AddressCards';
import AddressForm from './components/AddressForm';
import BulkImportModal from './components/BulkImportModal';
import Loading from './components/Loading';
import Toast from './components/Toast';

export default function HomePage() {
  const [addresses, setAddresses] = useState<Address[]>([]);
  const [filteredAddresses, setFilteredAddresses] = useState<Address[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);
  const [showAddModal, setShowAddModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showBulkImport, setShowBulkImport] = useState(false);
  const [selectedAddress, setSelectedAddress] = useState<Address | undefined>();
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' | 'info' } | null>(null);

  // Track the current search term to prevent race conditions
  const currentSearchRef = useRef('');

  

  // Client-side search helper
  const searchInAddress = useCallback((address: Address, term: string): boolean => {
    const searchLower = term.toLowerCase();
    
    if (address.city.toLowerCase().includes(searchLower)) return true;
    if (address.region.toLowerCase().includes(searchLower)) return true;
    if (address.street.toLowerCase().includes(searchLower)) return true;
    if (address.branchNumber.toLowerCase().includes(searchLower)) return true;
    if (address.branchType.toLowerCase().includes(searchLower)) return true;
    if (address.phone.includes(term)) return true;
    
    const branchFull = `${address.branchType} ${address.branchNumber}`.toLowerCase();
    if (branchFull.includes(searchLower)) return true;
    
    const branchShort = address.branchType.includes('Поштомат') ? 'поштомат' : 'відд';
    const branchWithShort = `${branchShort} ${address.branchNumber}`.toLowerCase();
    if (branchWithShort.includes(searchLower)) return true;
    
    const location = `${address.city} ${address.region}`.toLowerCase();
    if (location.includes(searchLower)) return true;
    
    return false;
  }, []);

  // Filter addresses with proper backend integration
  useEffect(() => {
    // Update the current search reference
    currentSearchRef.current = searchTerm;

    // Empty search - show all
    if (searchTerm.trim() === '') {
      setFilteredAddresses(addresses);
      return;
    }

    // Client-side filtreleme (instant feedback)
    const filtered = addresses.filter((addr) => searchInAddress(addr, searchTerm));
    setFilteredAddresses(filtered);

    // Backend araması için debounce (3+ karakter için)
    if (searchTerm.trim().length >= 3) {
      const timeoutId = setTimeout(async () => {
        // Capture the search term at the time of the request
        const searchTermAtRequest = searchTerm;
        
        try {
          const results = await addressService.search(searchTermAtRequest);
          
          // KRITIK: Sadece bu hala aktif arama terimi ise sonucu uygula
          if (currentSearchRef.current === searchTermAtRequest) {
            setFilteredAddresses(results);
          }
          // Eğer kullanıcı aramayı değiştirdiyse bu sonucu YOKSAY
        } catch (error) {
          console.error('Backend search error:', error);
          // Hata durumunda client-side sonuçları kullan (zaten set edilmiş)
        }
      }, 500);

      return () => clearTimeout(timeoutId);
    }
  }, [searchTerm, addresses, searchInAddress]);

  const loadAddresses = useCallback(async () => {
    try {
      setLoading(true);
      const data = await addressService.getAll();
      setAddresses(data);
      setFilteredAddresses(data);
    } catch (error) {
      console.error('Error loading addresses:', error);
      showToast('Помилка завантаження адрес', 'error');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadAddresses();
  }, [loadAddresses]);

  const handleSearch = (term: string) => {
    setSearchTerm(term);
  };

  const handleCreate = async (data: CreateAddressDto) => {
    try {
      await addressService.create(data);
      showToast('Адресу успішно додано', 'success');
      setShowAddModal(false);
      await loadAddresses();
    } catch (error) {
      console.error('Error creating address:', error);
      showToast('Помилка при додаванні адреси', 'error');
    }
  };

  const handleEdit = (address: Address) => {
    setSelectedAddress(address);
    setShowEditModal(true);
  };

  const handleUpdate = async (data: UpdateAddressDto) => {
    if (!selectedAddress) return;

    try {
      await addressService.update(selectedAddress.id, data);
      showToast('Адресу успішно оновлено', 'success');
      setShowEditModal(false);
      setSelectedAddress(undefined);
      await loadAddresses();
    } catch (error) {
      console.error('Error updating address:', error);
      showToast('Помилка при оновленні адреси', 'error');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Ви впевнені, що хочете видалити цю адресу?')) return;

    try {
      await addressService.delete(id);
      showToast('Адресу успішно видалено', 'success');
      await loadAddresses();
    } catch (error) {
      console.error('Error deleting address:', error);
      showToast('Помилка при видаленні адреси', 'error');
    }
  };

  const handleBulkImport = async (texts: string[]) => {
    const result = await addressService.bulkImport(texts);
    await loadAddresses();
    return result;
  };

  const showToast = (message: string, type: 'success' | 'error' | 'info') => {
    setToast({ message, type });
  };

  return (
    <main className="min-h-screen bg-gray-50">
      <div className="container mx-auto px-4 py-6 max-w-7xl">
        {/* Header */}
        <header className="mb-6">
          <h1 className="text-2xl md:text-3xl font-bold text-gray-800 mb-4">
            Відділення Нової Пошти в місті Одеса
          </h1>

          <div className="flex flex-col sm:flex-row gap-3">
            <SearchBar
              value={searchTerm}
              onChange={handleSearch}
              placeholder="Пошук за містом, адресою, телефоном..."
            />

            <div className="flex gap-2">
              <button
                onClick={() => setShowAddModal(true)}
                className="flex items-center gap-2 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors whitespace-nowrap"
              >
                <Plus className="w-5 h-5" />
                <span className="hidden sm:inline">Додати</span>
              </button>

              <button
                onClick={() => setShowBulkImport(true)}
                className="flex items-center gap-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors whitespace-nowrap"
              >
                <Upload className="w-5 h-5" />
                <span className="hidden sm:inline">Імпорт</span>
              </button>
            </div>
          </div>

          {/* Results count */}
          <p className="mt-3 text-sm text-gray-600">
            Знайдено адрес: <span className="font-semibold">{filteredAddresses.length}</span>
            {searchTerm && addresses.length > 0 && (
              <span className="text-gray-500"> (з {addresses.length})</span>
            )}
          </p>
        </header>

        {/* Content */}
        {loading ? (
          <Loading />
        ) : (
          <>
            {/* Desktop Table View */}
            <AddressList
              addresses={filteredAddresses}
              onEdit={handleEdit}
              onDelete={handleDelete}
            />

            {/* Mobile Card View */}
            <AddressCards
              addresses={filteredAddresses}
              onEdit={handleEdit}
              onDelete={handleDelete}
            />
          </>
        )}

        {/* Modals */}
        {showAddModal && (
          <AddressForm
            onSubmit={handleCreate}
            onClose={() => setShowAddModal(false)}
          />
        )}

        {showEditModal && selectedAddress && (
          <AddressForm
            address={selectedAddress}
            onSubmit={handleUpdate}
            onClose={() => {
              setShowEditModal(false);
              setSelectedAddress(undefined);
            }}
          />
        )}

        {showBulkImport && (
          <BulkImportModal
            onImport={handleBulkImport}
            onClose={() => setShowBulkImport(false)}
          />
        )}

        {/* Toast Notification */}
        {toast && (
          <Toast
            message={toast.message}
            type={toast.type}
            onClose={() => setToast(null)}
          />
        )}
      </div>
    </main>
  );
}