import { test, expect } from '@playwright/test';
import * as actions from '../../TestSteps/ReusableTestSteps';
import * as login from '../../TestSteps/LoginbyRole';

test.beforeEach('Login User',async ({ page }) => {
    login.LoginbyRole(page, login.Users.Admin);
});

test.afterEach('Logout User',async ({ page }) => {
    await actions.click_button(page, 'logout');
});

test('Create a new User', async ({page}) =>{
    await actions.LaunchProject(page, 'Demo Project With Data' );
    const Random = Math.floor(Math.random() * 1000000);

    const user_name = 'user123' + Random;
    await actions.click_element(page, 'admin_menu');
    await actions.click_element(page, 'manage_users');

    await actions.click_button(page, 'users_create');
    
    await actions.fill_input(page, 'user_username', user_name);

    await actions.fill_input(page, 'user_password', 'sUPER_SECRET_PASSWORD123!');
    await actions.fill_input(page, 'user_email', 'playwright' + Random + '@example.com');

    await actions.select_dropdown_option(page, 'user_role', 'User');
    await actions.check_checkbox(page, 'email_confirmed');
    await actions.submit_form(page);

    await actions.validate_button(page, 'users_create');

    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').click();
    await page.getByLabel('Name filter value').click();
    await page.getByLabel('Name filter value').fill(user_name);
    await page.getByRole('button', { name: 'Apply' }).click();
    await expect(page.locator('tr', { hasText: user_name })).toBeVisible();

    await page.waitForSelector('.rz-tooltip.rz-popup', { state: 'hidden', timeout: 5000 });

    await actions.click_button(page, 'users_delete');

    await expect(page.locator('#rz-dialog-0-label')).toContainText('Delete ' + user_name );
    await page.getByRole('button', { name: 'Ok' }).first().click();
    await expect(page.getByRole('table')).not.toContainText(user_name);
    
})