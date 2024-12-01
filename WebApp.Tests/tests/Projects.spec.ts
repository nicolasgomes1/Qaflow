import { test, expect } from '@playwright/test';


const URL = 'https://localhost:7089';

test.beforeEach('Login User',async ({ page }) => {
    await page.goto(URL + '/Account/Login');
    await page.getByTestId('login_emailform').fill('admin@example.com');
    await page.getByTestId('login_passwordform').fill('Admin123!');
    await page.getByTestId('login_submitform').click();
    const heading = page.locator('strong', { hasText: 'Welcome, admin@example.com!'});
    await expect(heading).toBeVisible();
});

test.afterEach('Logout User',async ({ page }) => {
        await page.getByTestId('logout').click();
        const guest_user = page.locator('strong', { hasText: 'Welcome, Guest User!'});
        await expect(guest_user).toBeVisible();
});  


test('Create a new Project', async ({ page }) => {
    await page.getByTestId('create_project').click();
    const heading = page.locator('h3', { hasText: 'Create New Project' });
    await expect(heading).toBeVisible();
    
    await page.getByTestId('project_name').fill('Test Project');
    await page.getByTestId('project_description').fill('Test Project Description');
    await page.getByTestId('submit').click();
    const heading1 = page.locator('strong', { hasText: 'Welcome, admin@example.com!'});
    await expect(heading1).toBeVisible();
});