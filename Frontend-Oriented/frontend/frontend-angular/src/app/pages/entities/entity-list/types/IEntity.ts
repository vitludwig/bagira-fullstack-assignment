import { EEntityType } from '../../../../common/types/EEntityType';
import { ETaskForce } from '../../../../common/types/ETaskForce';

export interface IEntity {
  id: string;
  scenarioId: string;
  type: EEntityType;
  taskForce: ETaskForce;
  name: string;
  latitude: number;
  longitude: number;
  updatedAt: string | null;
}
