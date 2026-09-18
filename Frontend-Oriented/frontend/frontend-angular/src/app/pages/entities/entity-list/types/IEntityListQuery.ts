export interface IEntityListQuery {
  page: number;
  pageSize: number;
  search: string;
  type: string;
  taskForce: string;
  sortBy: string;
  sortDirection: 'asc' | 'desc';
}
