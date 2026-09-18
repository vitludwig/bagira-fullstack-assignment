import { expect, Page, test } from '@playwright/test';

const scenario = {
  id: 'scenario-1',
  name: 'Alpha Mission',
  description: 'Northern exercise',
  createdAt: '2026-09-18T08:00:00Z',
  updatedAt: '2026-09-19T08:00:00Z',
};

const entities = [
  {
    id: 'entity-1',
    scenarioId: 'scenario-1',
    type: 'Soldier',
    taskForce: 'Friendly',
    name: 'Falcon 1',
    latitude: 50.0755,
    longitude: 14.4378,
    updatedAt: '2026-09-19T08:00:00Z',
  },
  {
    id: 'entity-2',
    scenarioId: 'scenario-1',
    type: 'Vehicle',
    taskForce: 'Hostile',
    name: 'Viper 2',
    latitude: 49.1951,
    longitude: 16.6068,
    updatedAt: '2026-09-19T08:00:00Z',
  },
];

async function mockScenarioDetails(page: Page): Promise<void> {
  await page.route('http://localhost:5000/api/scenarios/scenario-1', async (route) => {
    await route.fulfill({ json: scenario });
  });
  await page.route('http://localhost:5000/api/scenarios/scenario-1/entities?*', async (route) => {
    await route.fulfill({ json: { items: entities, totalCount: entities.length } });
  });
  await page.route('https://tile.openstreetmap.org/**', async (route) => route.abort());
}

test('toggles entities between table and map views', async ({ page }) => {
  await mockScenarioDetails(page);
  await page.goto('/scenarios/scenario-1');

  await expect(page.getByRole('heading', { name: 'Alpha Mission' })).toBeVisible();
  await expect(page.getByRole('cell', { name: 'Falcon 1' })).toBeVisible();
  await expect(page.getByRole('cell', { name: 'Viper 2' })).toBeVisible();

  await page.getByRole('radio', { name: 'Map' }).click();
  await expect(page.locator('.leaflet-container')).toBeVisible();
  await expect(page.locator('path.leaflet-interactive')).toHaveCount(2);
  await expect(page.getByRole('cell', { name: 'Falcon 1' })).toBeHidden();

  await page.getByRole('radio', { name: 'Table' }).click();
  await expect(page.getByRole('cell', { name: 'Falcon 1' })).toBeVisible();
});

test('validates and creates an entity', async ({ page }) => {
  let requestBody: unknown;

  await mockScenarioDetails(page);
  await page.route('http://localhost:5000/api/scenarios/scenario-1/entities', async (route) => {
    requestBody = route.request().postDataJSON();
    await route.fulfill({ status: 201, json: { ...entities[0], id: 'entity-created' } });
  });

  await page.goto('/scenarios/scenario-1/entities/create');
  await page.getByRole('button', { name: 'Create entity' }).click();
  await expect(page.getByText('Entity type is required.')).toBeVisible();
  await expect(page.getByText('Task force is required.')).toBeVisible();

  await page.getByLabel('Entity type').click();
  await page.getByRole('option', { name: 'Soldier' }).click();
  await page.getByLabel('Task force').click();
  await page.getByRole('option', { name: 'Friendly' }).click();
  await page.getByLabel('Name / callsign').fill('  Falcon 3  ');
  await page.getByLabel('Latitude').fill('50.0755');
  await page.getByLabel('Longitude').fill('14.4378');
  await page.getByRole('button', { name: 'Create entity' }).click();

  await expect(page).toHaveURL(/\/scenarios\/scenario-1$/);
  expect(requestBody).toEqual({
    type: 'Soldier',
    taskForce: 'Friendly',
    name: 'Falcon 3',
    latitude: 50.0755,
    longitude: 14.4378,
  });
});
