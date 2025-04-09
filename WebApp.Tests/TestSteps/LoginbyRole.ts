import { expect, Page } from '@playwright/test';
/**
 * @param {Page} page - The Playwright page object.
 * @param {Enumerator} Users - User selected by role. Admin, Manager, User
 */
async function LoginbyRole(page: Page, selectedUser: Users)
{
    await page.goto('/');
    await page.waitForLoadState('networkidle');
    await expect(page.getByTestId('login')).toHaveText('Login');
    await page.goto('/Account/Login');

    
    if (selectedUser === Users.Admin) {
        await page.getByTestId('login_emailform').fill('admin@example.com');
        await page.getByTestId('login_passwordform').fill('Admin123!');
        await page.getByTestId('login_submitform').click();
        const heading = page.locator('strong', { hasText: 'Welcome, admin@example.com!'});
        await expect(heading).toBeVisible();
    }

    if (selectedUser === Users.Manager) {
        await page.getByTestId('login_emailform').fill('manager@example.com');
        await page.getByTestId('login_passwordform').fill('Manager123!');
        await page.getByTestId('login_submitform').click();
        const heading = page.locator('strong', { hasText: 'Welcome, manager@example.com!'});
        await expect(heading).toBeVisible();
    }

    if (selectedUser === Users.User) {
        await page.getByTestId('login_emailform').fill('user@example.com');
        await page.getByTestId('login_passwordform').fill('User123!');
        await page.getByTestId('login_submitform').click();
        const heading = page.locator('strong', { hasText: 'Welcome, user@example.com!'});
        await expect(heading).toBeVisible();
    }
    
}

enum Users {
    User,
    Manager,
    Admin
}

export { LoginbyRole, Users };