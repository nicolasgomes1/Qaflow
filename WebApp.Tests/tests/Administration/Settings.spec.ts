import { test, expect, Page } from '@playwright/test';
import * as actions from '../../TestSteps/ReusableTestSteps';
import * as login from '../../TestSteps/LoginbyRole';

/**
 * Ensure an integration is in the desired state
 * @param page Playwright page
 * @param integrationName Name of the integration row (e.g., 'OwnIntegrations')
 * @param enable true to enable, false to disable
 */
export async function setIntegrationState(page: Page, integrationName: string, enable: boolean) {
    const row = page.locator('tr', { hasText: integrationName });
    await expect(row).toBeVisible();

    // Check current status
    const rowText = await row.textContent();
    const isCurrentlyEnabled = rowText?.includes('Enabled');

    // If already in desired state, skip
    if ((enable && isCurrentlyEnabled) || (!enable && !isCurrentlyEnabled)) {
        return;
    }

    // Otherwise, toggle
    const checkbox = row.getByTestId('checkbox');

    await row.getByTestId('edit_button').click();
    await expect(checkbox).toBeVisible();

    await checkbox.click(); // works for custom checkbox
    await row.getByTestId('save_button').click();

    // Wait for the row text to reflect the new state
    await expect(row).toContainText(enable ? 'Enabled' : 'Disabled');
}

// =====================
// Example Test
// =====================

test.beforeEach('Login User', async ({ page }) => {
    await login.LoginbyRole(page, login.Users.Admin);
});

test.afterEach('Logout User', async ({ page }) => {
    await actions.click_button(page, 'logout');
});

test('Enable Own Integration if needed', async ({ page }) => {
    await page.waitForLoadState('networkidle');

    await actions.click_element(page, 'admin_menu');
    await actions.click_element(page, 'manage_settings');

    // Ensure integration is enabled
    await setIntegrationState(page, 'OwnIntegrations', true);
    await setIntegrationState(page, 'ExternalIntegrations', true);
    
    await page.reload();

    await actions.click_element(page, 'admin_menu');

    expect(page.getByTestId('manage_api')).toBeVisible();
    expect(page.getByTestId('manage_integrations')).toBeVisible();
    // Continue with next test steps...
});

test('Disable Own Integration if needed', async ({ page }) => {
    await page.waitForLoadState('networkidle');

    await actions.click_element(page, 'admin_menu');
    await actions.click_element(page, 'manage_settings');

    // Ensure integration is enabled
    await setIntegrationState(page, 'OwnIntegrations', false);
    await setIntegrationState(page, 'ExternalIntegrations', false);

    await page.reload();

    await actions.click_element(page, 'admin_menu');

    expect(page.getByTestId('manage_api')).not.toBeVisible();
    expect(page.getByTestId('manage_integrations')).not.toBeVisible();
    // Continue with next test steps...
});
