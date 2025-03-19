import { test, expect } from '@playwright/test';


test('Verify that Login and Register is visible', async ({ page }) => {
  test.slow();
  await page.goto('/');
  const guest_user = page.locator('strong', { hasText: 'Welcome, Guest User!'});
  await expect(guest_user).toBeVisible();
  // Expect a title "to contain" a substring.
  await expect(page.getByTestId('login')).toHaveText('Login');
  await expect(page.getByTestId('register')).toHaveText('Register');
  await expect(page.getByTestId('login_top')).toHaveText('Login');
  await expect(page.getByTestId('register_top')).toHaveText('Register');
});

test('Verify that Login page is accessible', async ({ page }) => {
  test.slow();
  await page.goto('/');

  // Click login
  await page.getByTestId('login').click();

  // Wait for navigation and specific content to load
  await page.waitForURL('/Account/Login');
  const heading = page.locator('h1', { hasText: 'Log in' });
  await expect(heading).toBeVisible();
});

test('Verify that Login page is accessible via Top navigation', async ({ page }) => {
  test.slow();

  await page.goto('/');
  await page.getByTestId('login_top').click();
  await page.waitForURL('/Account/Login');
  const heading = page.locator('h1', { hasText: 'Log in' });
  await expect(heading).toBeVisible();
});

test('Verify that Register page is accessible', async ({ page }) => {
  test.slow();

  await page.goto('/');

  // Click register
  await page.getByTestId('register').click();

  // Wait for navigation and specific content to load
  await page.waitForURL('/Account/Register');
  const heading = page.locator('h1', { hasText: 'Register' });
  await expect(heading).toBeVisible();
});

test('Verify that Register page is accessible via Top navigation', async ({ page }) => {
  test.slow();

  await page.goto('/');

  // Click register
  await page.getByTestId('register_top').click();

  // Wait for navigation and specific content to load
  await page.waitForURL('/Account/Register');
  const heading = page.locator('h1', { hasText: 'Register' });
  await expect(heading).toBeVisible();
});

test('User can Login as Admin', async ({ page }) => {
  test.slow();

  await page.goto('/Account/Login');

  await page.getByTestId('login_emailform').fill('admin@example.com');
  await page.getByTestId('login_passwordform').fill('Admin123!');
  await page.getByTestId('login_submitform').click();
  const heading = page.locator('strong', { hasText: 'Welcome, admin@example.com!'});
  await expect(heading).toBeVisible();
});

test('User caan Login as User', async ({ page }) => {
  test.slow();

  await page.goto('/Account/Login');

  await page.getByTestId('login_emailform').fill('user@example.com');
  await page.getByTestId('login_passwordform').fill('User123!');
  await page.getByTestId('login_submitform').click();
  const heading = page.locator('strong', { hasText: 'Welcome, user@example.com!'});
  await expect(heading).toBeVisible();
});

test('User can Login as Manager', async ({ page }) => {
  test.slow();
  await page.goto('/Account/Login');
  await page.getByTestId('login_emailform').fill('manager@example.com');
  await page.getByTestId('login_passwordform').fill('Manager123!');
  await page.getByTestId('login_submitform').click();
  const heading = page.locator('strong', { hasText: 'Welcome, manager@example.com!'});
  await expect(heading).toBeVisible();
});

test('User Can Loggout from the application', async ({ page }) => {
  test.slow();

  await page.goto('/Account/Login');
  await page.getByTestId('login_emailform').fill('manager@example.com');
  await page.getByTestId('login_passwordform').fill('Manager123!');
  await page.getByTestId('login_submitform').click();
  const heading = page.locator('strong', { hasText: 'Welcome, manager@example.com!'});
  await expect(heading).toBeVisible();
  await page.getByTestId('logout').click();
  const guest_user = page.locator('strong', { hasText: 'Welcome, Guest User!'});
  await expect(guest_user).toBeVisible();
});

test.skip('User Can Register in the application', async ({page}) => {
  // TODO
  test.slow();
  await page.goto('/Account/Register');

  await page.getByTestId('register_emailform').fill('nicolasdiasgomes+10@gmail.com');
  await page.getByTestId('register_passwordform').fill('User@1000!');
  await page.getByTestId('register_confirmpasswordform').fill('User@1000!');
  await page.getByTestId('register_submitform').click();

  const confirmation = page.getByText("Please check your email to confirm your account.")
  await expect(confirmation).toBeVisible();
})

test("User Registered can't register again", async ({page}) => {
  // TODO
  test.slow();
  await page.goto('/Account/Register');

  await page.getByTestId('register_emailform').fill('admin@example.com');
  await page.getByTestId('register_passwordform').fill('Admin123!');
  await page.getByTestId('register_confirmpasswordform').fill('Admin123!');
  await page.getByTestId('register_submitform').click();

  const confirmation = page.getByText("Error: Username 'admin@example.com' is already taken.\n")
  await expect(confirmation).toBeVisible();
})


