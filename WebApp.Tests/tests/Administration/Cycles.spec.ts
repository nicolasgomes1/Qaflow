import { test, expect } from '@playwright/test';
import * as actions from '../../TestSteps/ReusableTestSteps';
import * as login from '../../TestSteps/LoginbyRole';

test.beforeEach('Login User',async ({ page }) => {
    login.LoginbyRole(page, login.Users.Admin);
});

test.afterEach('Logout User',async ({ page }) => {
    await actions.click_button(page, 'logout');
});

test('Create a new Cycle', async ({ page }) => {

    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_element(page, 'admin_menu');
    await actions.click_element(page, 'manage_cycles');


    const Random = Math.floor(Math.random() * 1000000);

    await actions.click_button(page, 'cycles_create');

    const cycle_name = 'Cycle Playwright' + Random;
    await actions.fill_input(page, 'cycle_name', cycle_name);

    await actions.fill_date_picker(page, 'cycle_start_date', '2024-03-20');
    
    await actions.closeModal(page, "submit_dialog");

    await actions.validate_button(page, 'cycles_create');

    if (await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()) {
        await page.locator('.rz-notification-item > div:nth-child(2)').click();
    }

    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').click();
    await page.getByLabel('Name filter value').click();
    await page.getByLabel('Name filter value').fill(cycle_name);
    await page.getByRole('button', { name: 'Apply' }).click();
    await expect(page.locator('tr', { hasText: cycle_name })).toBeVisible();

    await page.waitForSelector('.rz-tooltip.rz-popup', { state: 'hidden', timeout: 5000 });
    
    await actions.click_button(page, 'delete');



    await expect(page.locator('#rz-dialog-0-label')).toContainText('Delete Cycles');
    await page.getByRole('button', { name: 'Ok' }).first().click();
    await expect(page.getByRole('table')).not.toContainText(cycle_name);

});

test('Cycle with linked entities cannot be deleted or edited', async ({ page }) => {
    const cycle_name = 'Cycle 1';
    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_element(page, 'admin_menu');
    await actions.click_element(page, 'manage_cycles');

    
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').click();
    await page.getByLabel('Name filter value').click();
    await page.getByLabel('Name filter value').fill(cycle_name);
    await page.getByRole('button', { name: 'Apply' }).click();
    await expect(page.locator('tr', { hasText: cycle_name })).toBeVisible();

    await page.waitForSelector('.rz-tooltip.rz-popup', { state: 'hidden', timeout: 5000 });

    await actions.validate_button_disabled(page, 'delete');
    await actions.validate_button_disabled(page, 'edit');
});
