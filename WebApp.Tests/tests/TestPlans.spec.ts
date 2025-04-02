import { test, expect, Page } from '@playwright/test';
import * as actions from '../TestSteps/ReusableTestSteps';
import * as login from '../TestSteps/LoginbyRole';
import * as filter from '../TestSteps/FilterSteps';

test.beforeEach('Login User',async ({ page }) => {
    login.LoginbyRole(page, login.Users.Admin);

});

test.afterEach('Logout User',async ({ page }) => {
    await actions.click_button(page, 'logout');
    const guest_user = page.locator('strong', { hasText: 'Welcome, Guest User!'});
    await expect(guest_user).toBeVisible();
});

test('Create New Test Plan With Main Information', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);
    const name = 'Test Plan Playwright' + Random;
    const description = 'Test Test Plan Playwright Description' + Random;
    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_testplans');

    await actions.click_button(page, 'create_testplan');

    await actions.fill_input(page, 'testplan_name', name);

    await actions.fill_input(page, 'testplan_description', description);

    await actions.select_dropdown_option(page, 'testplan_priority', 'Medium');


    await actions.select_dropdown_option(page,'testplan_status', 'New');

    await actions.select_dropdown_option(page,'testplan_assignedto', 'user@example.com');

    await actions.submit_form(page);

    await actions.validate_button(page, 'create_testplan');

    await filter.filterTableModel(page, name);

    await actions.click_button(page, 'delete');

    await page.getByRole('button', { name: 'Ok' }).click();
    await expect(page.getByRole('table')).not.toContainText(name);
});

test('Create New Test Plan With Files', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);
    const name = 'Test Plan Playwright' + Random;
    const description = 'Test Test Plan Playwright Description' + Random;
    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_testplans');

    await actions.click_button(page, 'create_testplan');

    await actions.fill_input(page, 'testplan_name', name);

    await actions.fill_input(page, 'testplan_description', description);

    await actions.select_dropdown_option(page, 'testplan_priority', 'Medium');


    await actions.select_dropdown_option(page,'testplan_status', 'New');

    await actions.select_dropdown_option(page,'testplan_assignedto', 'user@example.com');

    await page.evaluate(() => document.activeElement && (document.activeElement as HTMLElement).blur());

    await actions.ClickTab(page, 'testplan_files');

    await actions.UploadFile(page, 'fileupload');
    
    await actions.submit_form(page);

    await actions.validate_button(page, 'create_testplan');

    await filter.filterTableModel(page, name);

    await actions.click_button(page, 'view');
    await actions.ClickTab(page, 'testplan_files');
    await actions.validate_page_has_text(page, 'testfile.png');
});