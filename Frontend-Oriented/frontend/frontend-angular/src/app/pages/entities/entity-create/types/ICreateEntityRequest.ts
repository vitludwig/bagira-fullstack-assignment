import { EEntityType } from '../../../../common/types/EEntityType';
import { ETaskForce } from '../../../../common/types/ETaskForce';

export interface ICreateEntityRequest {
  type: EEntityType;
  taskForce: ETaskForce;
  name: string;
  latitude: number;
  longitude: number;
}
