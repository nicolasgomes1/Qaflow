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
    await page.waitForLoadState('networkidle');
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

async function closeModal(page: Page)
{
    await page.evaluate(() => {
        (function() {
            const selector = '[data-testid="submit_dialog"]'; // Replace with your selector

            const element = document.querySelector(selector);
            if (!element) {
                console.error(`Element with data-testid="${selector}" not found.`);
                return;
            }

            // 1. Simulate Hover (to show the hand cursor)
            const mouseEnterEvent = new MouseEvent('mouseenter', {
                bubbles: true,
                cancelable: true,
                view: window
            });
            element.dispatchEvent(mouseEnterEvent);

            // 2. Simulate Click (mousedown + mouseup to trigger click)
            const mouseDownEvent = new MouseEvent('mousedown', {
                bubbles: true,
                cancelable: true,
                view: window
            });

            const mouseUpEvent = new MouseEvent('mouseup', {
                bubbles: true,
                cancelable: true,
                view: window
            });

            element.dispatchEvent(mouseDownEvent); // Simulate the mouse button being pressed
            element.dispatchEvent(mouseUpEvent);   // Simulate the mouse button being released

            // Optionally, trigger a final click event for browsers that require it
            const clickEvent = new MouseEvent('click', {
                bubbles: true,
                cancelable: true,
                view: window
            });
            element.dispatchEvent(clickEvent);
        })();
    });
}


test('Create New Requirement Specification', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000);

    await filterAndLaunchProject(page);

    await actions.click_button(page, 'd_requirementsSpecification');
    
    await actions.click_button(page, 'requirements_specification_create');

    const name = 'Test Requirement Playwright' + Random;
    await actions.fill_input(page, 'requirements_specification_name', name);
    await actions.validate_input(page, 'requirements_specification_name', name);

    const description ='Test Requirement Playwright Description' + Random;
    await actions.fill_input(page, 'requirements_specification_description', description);
    await actions.validate_input(page, 'requirements_specification_description', description);

await closeModal(page);
    await actions.validate_button(page, 'requirements_specification_create');
    
    await filterRequirements(page, 'Test Requirement Playwright' + Random);
    
    await actions.click_button(page, 'delete');

    await page.getByRole('button', { name: 'Ok' }).click();
    await expect(page.getByRole('table')).not.toContainText('Test Requirement Playwright' + Random);
});




test('View New Requirement Specification', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000);

    await filterAndLaunchProject(page);

    await actions.click_button(page, 'd_requirementsSpecification');

    await actions.click_button(page, 'requirements_specification_create');
    await page.waitForLoadState('networkidle')
    const name = 'Test Requirement Playwright' + Random;
    await actions.fill_input(page, 'requirements_specification_name', name);

    const description ='Test Requirement Playwright Description' + Random;
    await actions.fill_input(page, 'requirements_specification_description',description );

await closeModal(page);

    await actions.validate_button(page, 'requirements_specification_create');

    await filterRequirements(page, name);

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

