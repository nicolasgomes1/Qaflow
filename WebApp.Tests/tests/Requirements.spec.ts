import { test, expect, Page } from '@playwright/test';
import * as actions from '../TestSteps/ReusableTestSteps';
import * as login from '../TestSteps/LoginbyRole';

test.beforeEach('Login User',async ({ page }) => {
    login.LoginbyRole(page, login.Users.Admin);

});

test.afterEach('Logout User',async ({ page }) => {
    await actions.click_button(page, 'logout');
    const guest_user = page.locator('strong', { hasText: 'Welcome, Guest User!'});
    await expect(guest_user).toBeVisible();
});

async function filterAndLaunchProject(page: Page, projectName = 'Demo Project Without Data') {
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').click();
    await page.getByRole('textbox', { name: 'Name filter value' }).click();
    await page.getByRole('textbox', { name: 'Name filter value' }).fill(projectName);
    await page.getByRole('button', { name: 'Apply' }).click();
    await page.getByRole('button', { name: 'launch' }).first().click({ force: true });
    await page.waitForLoadState('load');
}

async function filterRequirements(page: Page, requirementName: string) {
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').click();
    await page.getByRole('textbox', { name: 'Name filter value' }).click();
    await page.getByRole('textbox', { name: 'Name filter value' }).fill(requirementName);
    await page.getByRole('button', { name: 'Apply' }).click();
    if (await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()) {
        await page.locator('.rz-notification-item > div:nth-child(2)').click();
    }
    await expect(page.getByRole('table')).toContainText(requirementName);
    await page.waitForLoadState('load');
}

test('Create New Requirement', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000);

    await filterAndLaunchProject(page);

    await actions.click_button(page, 'd_requirements');
    
    await actions.click_button(page, 'create_requirement');

    await actions.fill_input(page, 'requirement_name', 'Test Requirement Playwright' + Random);

    await actions.fill_input(page, 'requirement_description', 'Test Requirement Playwright Description' + Random);
    
    await actions.select_dropdown_option(page, 'requirement_priority', 'Medium');
    

    await actions.select_dropdown_option(page,'requirement_status', 'Completed');

    await actions.select_dropdown_option(page,'requirement_assignedto', 'user@example.com');
    
    await actions.submit_form(page);

    await actions.validate_button(page, 'create_requirement');
    
    await filterRequirements(page, 'Test Requirement Playwright' + Random);
    
    await actions.click_button(page, 'delete');

    await page.getByRole('button', { name: 'Ok' }).click();
    await expect(page.getByRole('table')).not.toContainText('Test Requirement Playwright' + Random);
});




test('View New Requirement', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000);

    await filterAndLaunchProject(page);

    await actions.click_button(page, 'd_requirements');
    await actions.click_button(page, 'create_requirement');
    const name = 'Test Requirement Playwright' + Random;
    await actions.fill_input(page, 'requirement_name', name);

    const description = 'Test Requirement Playwright Description' + Random;
    await actions.fill_input(page, 'requirement_description', description);

    await actions.select_dropdown_option(page, 'requirement_priority', 'Medium');
    await actions.select_dropdown_option(page,'requirement_status', 'Completed');
    await actions.select_dropdown_option(page,'requirement_assignedto', 'user@example.com');
    await actions.submit_form(page);
    await actions.validate_button(page, 'create_requirement');


    await filterRequirements(page, name);


    await actions.click_button(page, 'view');

    expect(page.getByText(name)).toBeVisible();
    expect(page.getByText(description)).toBeVisible();

});

test('Edit New Requirement', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000);

    await filterAndLaunchProject(page);

    await actions.click_button(page, 'd_requirements');
    await actions.click_button(page, 'create_requirement');
    const name = 'Test Requirement Playwright' + Random;
    await actions.fill_input(page, 'requirement_name', name);

    const description = 'Test Requirement Playwright Description' + Random;
    await actions.fill_input(page, 'requirement_description', description);

    await actions.select_dropdown_option(page, 'requirement_priority', 'Medium');
    await actions.select_dropdown_option(page,'requirement_status', 'Completed');
    await actions.select_dropdown_option(page,'requirement_assignedto', 'user@example.com');
    await actions.submit_form(page);
    await actions.validate_button(page, 'create_requirement');


    await filterRequirements(page, name);


    await actions.click_button(page, 'edit');
    await actions.validate_input(page, 'requirement_name', name);
    await actions.validate_input(page, 'requirement_description', description);

    await actions.submit_form(page);
    await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()

});