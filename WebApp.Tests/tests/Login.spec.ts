import { test, expect } from '@playwright/test';


const URL = 'https://localhost:7089';

test('Access Login', async ({ page }) => {
  await page.goto(URL);

  // Expect a title "to contain" a substring.
  await expect(page.getByTestId('login')).toHaveText('Login');
  await expect(page.getByTestId('register')).toHaveText('Register');
});

test('Click Login', async ({ page }) => {
  await page.goto(URL);

  // Click login
  await page.getByTestId('login').click();

  // Wait for navigation and specific content to load
  await page.waitForURL(URL + '/Account/Login');
  const heading = page.locator('h1', { hasText: 'Log in' });
  await expect(heading).toBeVisible();
});

test('Click Login top nav', async ({ page }) => {
  await page.goto(URL);
  await page.getByTestId('login_top').click();
  await page.waitForURL(URL + '/Account/Login');
  const heading = page.locator('h1', { hasText: 'Log in' });
  await expect(heading).toBeVisible();
});

test('Click Register', async ({ page }) => {
  await page.goto(URL);

  // Click register
  await page.getByTestId('register').click();

  // Wait for navigation and specific content to load
  await page.waitForURL(URL +'/Account/Register');
  const heading = page.locator('h1', { hasText: 'Register' });
  await expect(heading).toBeVisible();
});

test('Click Register top', async ({ page }) => {
  await page.goto(URL);

  // Click register
  await page.getByTestId('register_top').click();

  // Wait for navigation and specific content to load
  await page.waitForURL(URL +'/Account/Register');
  const heading = page.locator('h1', { hasText: 'Register' });
  await expect(heading).toBeVisible();
});

test('Submit Login as Admin', async ({ page }) => {
  await page.goto(URL + '/Account/Login');

  await page.getByTestId('login_emailform').fill('admin@example.com');
  await page.getByTestId('login_passwordform').fill('Admin123!');
  await page.getByTestId('login_submitform').click();
  const heading = page.locator('strong', { hasText: 'Welcome, admin@example.com!'});
  await expect(heading).toBeVisible();
});

test('Submit Login as User', async ({ page }) => {
  await page.goto(URL + '/Account/Login');

  await page.getByTestId('login_emailform').fill('user@example.com');
  await page.getByTestId('login_passwordform').fill('User123!');
  await page.getByTestId('login_submitform').click();
  const heading = page.locator('strong', { hasText: 'Welcome, user@example.com!'});
  await expect(heading).toBeVisible();
});

test('Submit Login as Manager', async ({ page }) => {
  await page.goto(URL + '/Account/Login');
  await page.getByTestId('login_emailform').fill('manager@example.com');
  await page.getByTestId('login_passwordform').fill('Manager123!');
  await page.getByTestId('login_submitform').click();
  const heading = page.locator('strong', { hasText: 'Welcome, manager@example.com!'});
  await expect(heading).toBeVisible();
});

test('User Can Loggout', async ({ page }) => {
  await page.goto(URL + '/Account/Login');
  await page.getByTestId('login_emailform').fill('manager@example.com');
  await page.getByTestId('login_passwordform').fill('Manager123!');
  await page.getByTestId('login_submitform').click();
  const heading = page.locator('strong', { hasText: 'Welcome, manager@example.com!'});
  await expect(heading).toBeVisible();
  await page.getByTestId('logout').click();
  const guest_user = page.locator('strong', { hasText: 'Welcome, Guest User!'});
  await expect(guest_user).toBeVisible();
});


