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


async function CreateBug(page: Page, number: number)
{
    const name = 'Test Bug Playwright' + number;
    await actions.fill_input(page, 'bug_name', name);

    const description = 'Test Bug Playwright Description' + number;
    await actions.fill_input(page, 'bug_description', description);

    await actions.select_dropdown_option(page, 'bug_priority', 'Medium');
    await actions.select_dropdown_option(page,'bug_status', 'Open');
    await actions.select_dropdown_option(page,'bug_wkfstatus', 'Completed');

    await actions.select_dropdown_option(page,'bug_assignedto', 'user@example.com');

    await actions.submit_form(page);

    await actions.validate_button(page, 'create_bug');
}

async function CreateBugWithFile(page: Page, number: number)
{
    const name = 'Test Bug Playwright' + number;
    await actions.fill_input(page, 'bug_name', name);

    const description = 'Test Bug Playwright Description' + number;
    await actions.fill_input(page, 'bug_description', description);

    await actions.select_dropdown_option(page, 'bug_priority', 'Medium');
    await actions.select_dropdown_option(page,'bug_status', 'Open');
    await actions.select_dropdown_option(page,'bug_wkfstatus', 'Completed');

    await actions.select_dropdown_option(page,'bug_assignedto', 'user@example.com');

    await page.evaluate(() => document.activeElement && (document.activeElement as HTMLElement).blur());

    await actions.ClickTab(page, 'bug_files');

    await actions.UploadFile(page, 'fileupload');
    
    await actions.submit_form(page);

    await actions.validate_button(page, 'create_bug');
}


test('Create New Bug', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);

    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_bugs');

    await actions.click_button(page, 'create_bug');


    await CreateBug(page, Random);
    const name = 'Test Bug Playwright' + Random;
    await filter.filterTableModel(page, name);

    await actions.click_button(page, 'delete');

    await page.getByRole('button', { name: 'Ok' }).click();
    await expect(page.getByRole('table')).not.toContainText(name);
});

test('Create New Bug with file', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);

    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_bugs');

    await actions.click_button(page, 'create_bug');


    await CreateBugWithFile(page, Random);
    const name = 'Test Bug Playwright' + Random;
    await filter.filterTableModel(page, name);

    await actions.click_button(page, 'view');

    await actions.ClickTab(page, 'bug_files');
    await actions.validate_page_has_text(page, 'testfile.png');
});


test('View New Bug', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);

    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_bugs');

    await actions.click_button(page, 'create_bug');


    await CreateBug(page, Random);
    const name = 'Test Bug Playwright' + Random;
    await filter.filterTableModel(page, name);

    await actions.click_button(page, 'view');

    const description = 'Test Bug Playwright Description' + Random;

    expect(page.getByText(name)).toBeVisible();
    expect(page.getByText(description)).toBeVisible();
});

test('Edit New Bug', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);

    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_bugs');

    await actions.click_button(page, 'create_bug');


    await CreateBug(page, Random);
    const name = 'Test Bug Playwright' + Random;
    const description = 'Test Bug Playwright Description' + Random;

    await filter.filterTableModel(page, name);

    await actions.click_button(page, 'edit');

    await actions.validate_input(page, 'bug_name', name);
    await actions.validate_input(page, 'bug_description', description);

    await actions.submit_form(page);
    await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()
})