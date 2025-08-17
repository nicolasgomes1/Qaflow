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


test('Create New Requirement Specification', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);

    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_requirementsSpecification');
    
    await actions.click_button(page, 'requirements_specification_create');

    const name = 'Test Requirement Playwright' + Random;
    
    await actions.fill_input_and_validate(page, 'requirements_specification_name', name);
    
    const description ='Test Requirement Playwright Description' + Random;

    await actions.fill_input_and_validate(page, 'requirements_specification_description', description);
    

    await actions.closeModal(page, "submit_dialog");
    await actions.validate_button(page, 'requirements_specification_create');
    
    await filter.filterTableModel(page, 'Test Requirement Playwright' + Random);
    
    await actions.click_button(page, 'delete');

    // Ensure the dialog is visible
    const dialog = page.locator('.rz-dialog');

    // Check if the dialog is visible
    await expect(dialog).toBeVisible();

    await page.getByRole('button', { name: 'Ok' }).first().click();
    await expect(page.getByRole('table')).not.toContainText('Test Requirement Playwright' + Random);
});


test('View New Requirement Specification', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000000);

    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_requirementsSpecification');

    await actions.click_button(page, 'requirements_specification_create');
    await page.waitForLoadState('networkidle')
    const name = 'Test Requirement Playwright' + Random;
    await actions.fill_input(page, 'requirements_specification_name', name);

    const description ='Test Requirement Playwright Description' + Random;
    await actions.fill_input(page, 'requirements_specification_description',description );

    await actions.closeModal(page, "submit_dialog");

    await actions.validate_button(page, 'requirements_specification_create');

    await filter.filterTableModel(page, name);

    await actions.click_button(page, 'view');

// Ensure the dialog is visible
    const dialog = page.locator('.rz-dialog');

// Check if the dialog is visible
    await expect(dialog).toBeVisible();

// Check if the name and description texts are visible within the dialog
    await expect(dialog.locator(`text=${name}`)).toBeVisible();
    await expect(dialog.locator(`text=${description}`)).toBeVisible();

    const close = page.locator('.rz-dialog-titlebar-close');
    await close.click({force: true});


    
});


test('Delete Requirement Specification', async ({ page })=> {

    test.slow();
    const Random = Math.floor(Math.random() * 1000000);

    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_requirementsSpecification');

    await actions.click_button(page, 'requirements_specification_create');
    await page.waitForLoadState('networkidle')
    const name = 'Test Requirement Playwright' + Random;
    await actions.fill_input(page, 'requirements_specification_name', name);

    const description ='Test Requirement Playwright Description' + Random;
    await actions.fill_input(page, 'requirements_specification_description',description );

    await actions.closeModal(page, "submit_dialog");

    await actions.validate_button(page, 'requirements_specification_create');

    await filter.filterTableModel(page, name);

    await actions.click_button(page, 'delete');

    // Ensure the dialog is visible
    const dialog = page.locator('.rz-dialog');

    // Check if the dialog is visible
    await expect(dialog).toBeVisible();

    await page.getByRole('button', { name: 'Ok' }).first().click();
    await expect(page.getByRole('table')).not.toContainText('Test Requirement Playwright' + Random);
})

test('Delete Requirement Specification is not possible with linked requirement', async ({ page })=> {

    test.slow();

    await actions.LaunchProject(page, 'Demo Project With Data' );

    await actions.click_button(page, 'd_requirementsSpecification');

    await page.waitForLoadState('networkidle')
    const name = 'Requirements Specification 1';


    await filter.filterTableModel(page, name);

    await actions.click_button(page, 'delete');

    // Ensure the dialog is visible
    const dialog = page.locator('.rz-dialog');

    // Check if the dialog is visible
    await expect(dialog).toBeVisible();

    await page.getByRole('button', { name: 'Ok' }).first().click();

    await expect(page.locator('.rz-notification-item').getByText("Can't delete Requirements Specification has it has children")).toBeVisible();
})