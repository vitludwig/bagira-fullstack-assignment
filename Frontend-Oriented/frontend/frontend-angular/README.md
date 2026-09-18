# Scenario Builder Frontend

Angular 21 frontend for managing training scenarios and their entities. The UI uses Angular Material and provides scenario and entity creation, server-side search, filtering, sorting, pagination, and table/map entity views.

## Development

Install dependencies and start the development server:

```bash
npm ci
npm start
```

The application runs at http://localhost:4200 and expects the API URL configured in `src/environments/environment.ts`.

Create a production build with:

```bash
npm run build
```

## UI tests

The Playwright tests start a separate Angular development server on http://127.0.0.1:4201 and mock API responses. The backend and PostgreSQL are not required.

Install the Chromium browser once:

```bash
npx playwright install chromium
```

Run the headless UI tests:

```bash
npm run test:e2e
```

Run Playwright's interactive UI:

```bash
npm run test:e2e:ui
```

Test reports and artifacts are generated in `playwright-report` and `test-results` and are ignored by Git.
