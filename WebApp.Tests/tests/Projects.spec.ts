import { test, expect } from '@playwright/test';
import * as actions from '../TestSteps/ReusableTestSteps';
import * as login from '../TestSteps/LoginbyRole';
import * as filter from '../TestSteps/FilterSteps';

test.beforeEach('Login User',async ({ page }) => {
    login.LoginbyRole(page, login.Users.Admin);
});

test.afterEach('Logout User',async ({ page }) => {
        await actions.click_button(page, 'logout');
});


test('Create a new Project', async ({ page }) => {
    const Random = Math.floor(Math.random() * 1000000);
    
    await actions.click_button(page, 'create_project');

    const project_name = 'Test Project Playwright' + Random;
    await actions.fill_input(page, 'project_name', project_name);

    await actions.fill_input(page, 'project_description', 'Sample my description');

    await actions.submit_form(page);

    await actions.validate_button(page, 'create_project');

    if (await page.locator('.rz-notification-item').isVisible()) {
        await page.getByRole('button', { name: 'Close' }).click();
    }

    await filter.filterTableModel(page, project_name);

    
    await page.getByRole('row', { name: project_name }).getByTestId('delete').click();


    await expect(page.getByText('All underlying Data will be')).toBeVisible();
    await page.getByRole('button', { name: 'Ok' }).click();
    await expect(page.getByRole('table')).not.toContainText(project_name);

});

test('View a new Project', async ({ page }) => {
    const Random = Math.floor(Math.random() * 1000000);

    await actions.click_button(page, 'create_project');

    const project_name = 'Test Project Playwright' + Random;
    await actions.fill_input(page, 'project_name', project_name);

    const description = 'Sample my description';
    await actions.fill_input(page, 'project_description', description);

    await actions.submit_form(page);

    await actions.validate_button(page, 'create_project');

    if (await page.locator('.rz-notification-item').isVisible()) {
        await page.getByRole('button', { name: 'Close' }).click();
    }

    await filter.filterTableModel(page, project_name);


    await page.getByRole('row', { name: project_name }).getByTestId('view').click();



    expect(page.getByText(project_name)).toBeVisible();
    expect(page.getByText(description)).toBeVisible();
    
});

test('Edit a new Project', async ({ page }) => {
    const Random = Math.floor(Math.random() * 1000000);

    await actions.click_button(page, 'create_project');

    const project_name = 'Test Project Playwright' + Random;
    await actions.fill_input(page, 'project_name', project_name);

    const description = 'Sample my description';
    await actions.fill_input(page, 'project_description', description);

    await actions.submit_form(page);

    await actions.validate_button(page, 'create_project');

    if (await page.locator('.rz-notification-item').isVisible()) {
        await page.getByRole('button', { name: 'Close' }).click();
    }

    await filter.filterTableModel(page, project_name);



    await page.getByRole('row', { name: project_name }).getByTestId('edit').click();

    await actions.validate_input(page, 'project_name', project_name);
    await actions.validate_input(page, 'project_description', description);
    

    await actions.submit_form(page);
    if (await page.locator('.rz-notification-item').isVisible()) {
        await page.getByRole('button', { name: 'Close' }).click();
    }

});

