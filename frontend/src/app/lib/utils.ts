export function formatBranchName(address: { branchType: string; branchNumber: string; city: string }): string {
  const type = address.branchType.includes('Поштомат') ? 'Поштомат' : 'Відд';
  return `${type} ${address.branchNumber}`;
}

export function cn(...classes: (string | undefined | null | boolean)[]): string {
  return classes.filter(Boolean).join(' ');
}