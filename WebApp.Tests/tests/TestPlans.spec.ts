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


test.describe('Test Plan Suite', () => {
    test('Create New Test Plan With Main Information', async ({ page })=> {
        test.slow();
        const Random = Math.floor(Math.random() * 1000000);
        const name = 'Test Plan Playwright' + Random;
        const description = 'Test Test Plan Playwright Description' + Random;
        await actions.LaunchProject(page, 'Demo Project With Data' );

        await actions.click_button(page, 'd_testplans');

        await actions.click_button(page, 'create_testplan');

        await actions.fill_input(page, 'testplan_name', name);

        await actions.fill_input(page, 'testplan_description', description);

        await actions.select_dropdown_option(page, 'testplan_priority', 'Medium');

        await select_dropdown_option_multi(page, "testplan_testcases", ["Test Case 2", "Test Case 5"]);


        await actions.select_dropdown_option(page, 'testplan_cycles', 'Cycle 1');

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
        await actions.LaunchProject(page, 'Demo Project With Data' );

        await actions.click_button(page, 'd_testplans');

        await actions.click_button(page, 'create_testplan');

        await actions.fill_input(page, 'testplan_name', name);

        await actions.fill_input(page, 'testplan_description', description);

        await actions.select_dropdown_option(page, 'testplan_priority', 'Medium');

        await select_dropdown_option_multi(page, "testplan_testcases", ["Test Case 1", "Test Case 2"]);

        await actions.select_dropdown_option(page, 'testplan_cycles', 'Cycle 1');


        await actions.select_dropdown_option(page,'testplan_status', 'New');

        await actions.select_dropdown_option(page,'testplan_assignedto', 'user@example.com');

        await page.getByTestId('testplan_files').click(); // Click on the "Files" tab using data-testid attribute


        await actions.UploadFile(page, 'fileupload');

        await actions.submit_form(page);

        await actions.validate_button(page, 'create_testplan');

        await filter.filterTableModel(page, name);

        await actions.click_button(page, 'view');
        await page.getByTestId('testplan_files').click(); // Click on the "Files" tab using data-testid attribute
        await actions.validate_page_has_text(page, 'testfile.png');
    });
})

