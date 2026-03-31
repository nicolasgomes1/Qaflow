import { test, expect, Page } from '@playwright/test';
import * as actions from '../TestSteps/ReusableTestSteps';
import * as login from '../TestSteps/LoginbyRole';
import * as filter from '../TestSteps/FilterSteps';

test.beforeEach('Login User',async ({ page }) => {
    login.LoginbyRole(page, login.Users.Admin);

});

test.afterEach('Logout User',async ({ page }) => {
    await actions.click_button(page, 'logout');
});



test('Create New Requirement', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);
    const requirement = 'Test Requirement Playwright' + Random;

    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_requirements');
    
    await actions.click_button(page, 'create_requirement');

    await actions.fill_input(page, 'requirement_name', requirement);

    await actions.fill_input(page, 'requirement_description', 'Test Requirement Playwright Description' + Random);
    
    await actions.select_dropdown_option(page, 'requirement_priority', 'Medium');
    

    await actions.select_dropdown_option(page,'requirement_status', 'New');

    await actions.select_dropdown_option(page,'requirement_assignedto', 'user@example.com');
    
    await actions.submit_form(page);

    if (await page.locator('.rz-notification-item').isVisible()) {
        await page.getByRole('button', { name: 'Close' }).click();
    }
    
    await actions.validate_button(page, 'create_requirement');
    
    await filter.filterTableModel(page, requirement);

    await page.getByRole('row', { name: requirement }).getByTestId('delete').click();

    await page.getByRole('button', { name: 'Ok' }).click();
    await expect(page.getByRole('table')).not.toContainText(requirement);
});




test('View New Requirement', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);

    await actions.LaunchProject(page, 'Demo Project Without Data' );

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

    if (await page.locator('.rz-notification-item').isVisible()) {
        await page.getByRole('button', { name: 'Close' }).click();
    }

    await filter.filterTableModel(page, name);


    await actions.click_button(page, 'view');

    expect(page.getByText(name)).toBeVisible();
    expect(page.getByText(description)).toBeVisible();

});

test('Edit New Requirement', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);

    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_requirements');
    await actions.click_button(page, 'create_requirement');
    const name = 'Test Requirement Playwright' + Random;
    await actions.fill_input(page, 'requirement_name', name);

    const description = 'Test Requirement Playwright Description' + Random;
    await actions.fill_input(page, 'requirement_description', description);

    await actions.select_dropdown_option(page, 'requirement_priority', 'Medium');
    await actions.select_dropdown_option(page,'requirement_status', 'New');
    await actions.select_dropdown_option(page,'requirement_assignedto', 'user@example.com');
    await actions.submit_form(page);
    await actions.validate_button(page, 'create_requirement');

    if (await page.locator('.rz-notification-item').isVisible()) {
        await page.getByRole('button', { name: 'Close' }).click();
    }

    await filter.filterTableModel(page, name);


    await page.getByRole('row', { name: name }).getByTestId('edit').click();


    await actions.validate_input(page, 'requirement_name', name);
    await actions.validate_input(page, 'requirement_description', description);

    await actions.submit_form(page);

});

test('Create New Requirement with file', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);

    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_requirements');

    await actions.click_button(page, 'create_requirement');

    const name = 'Test Requirement Playwright file' + Random;
    await actions.fill_input(page, 'requirement_name', name);

    await actions.fill_input(page, 'requirement_description', 'Test Requirement Playwright Description' + Random);

    await actions.select_dropdown_option(page, 'requirement_priority', 'Medium');


    await actions.select_dropdown_option(page,'requirement_status', 'New');

    await actions.select_dropdown_option(page,'requirement_assignedto', 'user@example.com');

    await page.evaluate(() => document.activeElement && (document.activeElement as HTMLElement).blur());

    await page.getByTestId('requirement_files').click(); // Click on the "Files" tab using data-testid attribute


    await actions.UploadFile(page, 'fileupload');
    
    await actions.submit_form(page);

    if (await page.locator('.rz-notification-item').isVisible()) {
        await page.getByRole('button', { name: 'Close' }).click();
    }
    
    await actions.validate_button(page, 'create_requirement');

    await filter.filterTableModel(page, name);
    await page.getByRole('row', { name: name }).getByTestId('view').click();

    await page.getByTestId('requirement_files').click(); // Click on the "Files" tab using data-testid attribute
    await actions.validate_page_has_text(page, 'testfile.png');


});


