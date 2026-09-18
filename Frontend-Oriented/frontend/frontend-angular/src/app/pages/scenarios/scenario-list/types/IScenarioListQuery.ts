export interface IScenarioListQuery {
  page: number;
  pageSize: number;
  search: string;
  sortBy: string;
  sortDirection: 'asc' | 'desc';
}
