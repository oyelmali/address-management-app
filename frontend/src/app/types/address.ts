export interface Address {
  id: number;
  region: string;
  city: string;
  branchNumber: string;
  branchType: string;
  street: string;
  phone: string;
  workingHoursWeekdays: string;
  workingHoursWeekend: string;
}

export interface CreateAddressDto {
  region: string;
  city: string;
  branchNumber: string;
  branchType: string;
  street: string;
  phone: string;
  workingHoursWeekdays: string;
  workingHoursWeekend: string;
}

export type UpdateAddressDto = CreateAddressDto;

export interface BulkImportResult {
  successCount: number;
  failureCount: number;
  errors: string[];
}