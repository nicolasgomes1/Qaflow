
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


test('Create New Test Case With Main Information', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);
    const testcase_name = 'Test Case Playwright' + Random;
    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_testcases');

    await actions.click_button(page, 'create_testcase');

    await actions.fill_input(page, 'testcase_name', testcase_name);

    await actions.fill_input(page, 'testcase_description', 'Test Test Cases Playwright Description' + Random);

    await actions.select_dropdown_option(page, 'testcase_priority', 'Medium');


    await actions.select_dropdown_option(page,'testcase_status', 'New');

    await actions.select_dropdown_option(page,'testcase_assignedto', 'user@example.com');

    await actions.submit_form(page);

    if (await page.locator('.rz-notification-item').isVisible()) {
        await page.getByRole('button', { name: 'Close' }).click();
    }
    
    await actions.validate_button(page, 'create_testcase');

    await filter.filterTableModel(page, testcase_name);

    await page.getByRole('row', { name: testcase_name }).getByTestId('delete').click();



    await page.getByRole('button', { name: 'Ok' }).click();

    if (await page.locator('.rz-notification-item').isVisible()) {
        await page.getByRole('button', { name: 'Close' }).click();
    }
    await expect(page.getByRole('table')).not.toContainText(testcase_name);
});

test('Create New Test Case With Files', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);
    const testcase_name = 'Test Case Playwright' + Random;
    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_testcases');

    await actions.click_button(page, 'create_testcase');

    await actions.fill_input(page, 'testcase_name', testcase_name);

    await actions.fill_input(page, 'testcase_description', 'Test Test Cases Playwright Description' + Random);

    await actions.select_dropdown_option(page, 'testcase_priority', 'Medium');


    await actions.select_dropdown_option(page,'testcase_status', 'New');

    await actions.select_dropdown_option(page,'testcase_assignedto', 'user@example.com');

    await page.getByTestId('testcase_files').click(); // Click on the "Files" tab using data-testid attribute


    await actions.UploadFile(page, 'fileupload');
    
    
    await actions.submit_form(page);

    if (await page.locator('.rz-notification-item').isVisible()) {
        await page.getByRole('button', { name: 'Close' }).click();
    }
    
    await actions.validate_button(page, 'create_testcase');

    await filter.filterTableModel(page, testcase_name);

    await page.getByRole('row', { name: testcase_name }).getByTestId('view').click();


    await page.getByTestId('testcase_files').click(); // Click on the "Files" tab using data-testid attribute
    await actions.validate_page_has_text(page, 'testfile.png');

});