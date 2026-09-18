import { IScenarioFormValue } from './IScenarioFormValue';

export interface ICreateScenarioRequest extends Omit<IScenarioFormValue, 'description'> {
  description: string | null;
}
