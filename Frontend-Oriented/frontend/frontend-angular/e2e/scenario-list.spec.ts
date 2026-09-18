import { expect, test } from '@playwright/test';

test('lists scenarios and searches without losing input focus', async ({ page }) => {
  const searches: string[] = [];

  await page.route('http://localhost:5000/api/scenarios?*', async (route) => {
    const search = new URL(route.request().url()).searchParams.get('search') ?? '';
    searches.push(search);
    const scenarios = [
      {
        id: 'scenario-1',
        name: 'Alpha Mission',
        description: 'Northern exercise',
        entityCount: 2,
        updatedAt: '2026-09-19T08:00:00Z',
      },
      {
        id: 'scenario-2',
        name: 'Bravo Mission',
        description: null,
        entityCount: 0,
        updatedAt: '2026-09-18T08:00:00Z',
      },
    ].filter((scenario) => scenario.name.toLowerCase().includes(search.toLowerCase()));

    await route.fulfill({ json: { items: scenarios, totalCount: scenarios.length } });
  });

  await page.goto('/scenarios');

  await expect(page.getByRole('heading', { name: 'Scenarios' })).toBeVisible();
  await expect(page.getByRole('link', { name: 'Alpha Mission' })).toBeVisible();
  await expect(page.getByRole('link', { name: 'Bravo Mission' })).toBeVisible();

  const search = page.getByRole('searchbox', { name: 'Search scenarios' });
  await search.fill('Alpha');
  await expect(search).toBeFocused();
  await expect.poll(() => searches).toContain('Alpha');
  await expect(page.getByRole('link', { name: 'Alpha Mission' })).toBeVisible();
  await expect(page.getByRole('link', { name: 'Bravo Mission' })).toBeHidden();
  await expect(search).toBeFocused();
});
