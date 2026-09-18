import { expect, test } from '@playwright/test';

test('validates and creates a scenario', async ({ page }) => {
  let requestBody: unknown;

  await page.route('http://localhost:5000/api/scenarios', async (route) => {
    requestBody = route.request().postDataJSON();
    await route.fulfill({
      status: 201,
      json: {
        id: 'scenario-created',
        name: 'Rescue Exercise',
        description: 'Mountain response training',
        createdAt: '2026-09-19T09:00:00Z',
        updatedAt: '2026-09-19T09:00:00Z',
      },
    });
  });
  await page.route('http://localhost:5000/api/scenarios/scenario-created', async (route) => {
    await route.fulfill({
      json: {
        id: 'scenario-created',
        name: 'Rescue Exercise',
        description: 'Mountain response training',
        createdAt: '2026-09-19T09:00:00Z',
        updatedAt: '2026-09-19T09:00:00Z',
      },
    });
  });
  await page.route(
    'http://localhost:5000/api/scenarios/scenario-created/entities?*',
    async (route) => route.fulfill({ json: { items: [], totalCount: 0 } }),
  );

  await page.goto('/scenarios/create');
  await page.getByRole('button', { name: 'Create scenario' }).click();
  await expect(page.getByText('Name is required.')).toBeVisible();

  await page.getByLabel('Scenario name').fill('  Rescue Exercise  ');
  await page.getByLabel('Description').fill('Mountain response training');
  await page.getByRole('button', { name: 'Create scenario' }).click();

  await expect(page).toHaveURL(/\/scenarios\/scenario-created$/);
  await expect(page.getByRole('heading', { name: 'Rescue Exercise' })).toBeVisible();
  expect(requestBody).toEqual({
    name: 'Rescue Exercise',
    description: 'Mountain response training',
  });
});
