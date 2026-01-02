/**
 * SPA-Like AJAX Handler - Zero Page Reloads
 * Handles ALL form submissions and navigation via AJAX like React
 */

(function() {
    'use strict';

    // ============================================
    // INTERCEPT ALL FORM SUBMISSIONS
    // ============================================
    
    document.addEventListener('submit', function(e) {
        const form = e.target;
        
        // Skip forms that explicitly opt-out
        if (form.hasAttribute('data-no-ajax')) {
            return;
        }

        e.preventDefault();
        handleFormSubmit(form);
    }, true);

    async function handleFormSubmit(form) {
        const formData = new FormData(form);
        const method = form.method?.toUpperCase() || 'POST';
        const action = form.action || window.location.href;
        const submitButton = form.querySelector('button[type="submit"], input[type="submit"]');
        const originalButtonText = submitButton ? submitButton.innerHTML : '';

        // Show loading state
        if (submitButton) {
            submitButton.disabled = true;
            submitButton.innerHTML = '<i class="fas fa-spinner fa-spin mr-2"></i>Processing...';
        }

        // Show page loading indicator
        showPageLoader();

        try {
            const response = await fetch(action, {
                method: method,
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const contentType = response.headers.get('content-type') || '';

            // Handle JSON response
            if (contentType.includes('application/json')) {
                const data = await response.json();
                handleJsonResponse(data, response.ok, form);
            }
            // Handle HTML response (partial or full page)
            else if (contentType.includes('text/html')) {
                const html = await response.text();
                
                // Check for redirect header
                const redirectUrl = response.headers.get('X-Redirect-Url');
                if (redirectUrl) {
                    await navigateTo(redirectUrl);
                } else if (response.redirected) {
                    await navigateTo(response.url);
                } else {
                    // Update page content
                    updatePageContent(html);
                    showToast('success', 'Operation completed successfully');
                }
            }
            // Handle redirect response
            else if (response.redirected) {
                await navigateTo(response.url);
            }

        } catch (error) {
            console.error('Form Submit Error:', error);
            showToast('error', 'An error occurred. Please try again.');
        } finally {
            hidePageLoader();
            if (submitButton) {
                submitButton.disabled = false;
                submitButton.innerHTML = originalButtonText;
            }
        }
    }

    function handleJsonResponse(data, isOk, form) {
        if (isOk && data.success !== false) {
            // Show success message
            if (data.message) {
                showToast('success', data.message);
            }

            // Handle redirect
            if (data.redirectUrl) {
                setTimeout(() => navigateTo(data.redirectUrl), 1000);
                return;
            }

            // Update specific element if specified
            if (data.updateTarget && data.html) {
                const target = document.querySelector(data.updateTarget);
                if (target) {
                    target.innerHTML = data.html;
                }
            }

            // Remove item from table/list
            if (data.removeId) {
                const element = document.querySelector(`[data-id="${data.removeId}"]`);
                if (element) {
                    element.style.transition = 'opacity 0.3s, transform 0.3s';
                    element.style.opacity = '0';
                    element.style.transform = 'translateX(-20px)';
                    setTimeout(() => element.remove(), 300);
                }
            }

            // Reset form if specified
            if (data.resetForm || form.hasAttribute('data-ajax-reset')) {
                form.reset();
            }

            // Close modal if in one
            const modal = form.closest('[data-modal], .modal');
            if (modal && data.closeModal !== false) {
                closeModal(modal);
            }

            // Trigger refresh callback
            if (data.refresh && typeof window.refreshPageData === 'function') {
                window.refreshPageData();
            }

            // Dispatch success event
            document.dispatchEvent(new CustomEvent('ajax:success', { detail: data }));

        } else {
            // Handle error
            let errorMessage = data.message || 'An error occurred';
            
            if (data.errors) {
                if (typeof data.errors === 'object') {
                    errorMessage = Object.values(data.errors).flat().join('<br>');
                }
                // Highlight invalid fields
                highlightValidationErrors(form, data.errors);
            }

            showToast('error', errorMessage);
            document.dispatchEvent(new CustomEvent('ajax:error', { detail: data }));
        }
    }

    // ============================================
    // SPA NAVIGATION - NO PAGE RELOADS
    // ============================================

    // Intercept all link clicks
    document.addEventListener('click', function(e) {
        const link = e.target.closest('a[href]');
        
        if (!link) return;
        
        const href = link.getAttribute('href');
        
        // Skip external links, anchors, javascript, and opt-out links
        if (!href || 
            href.startsWith('#') || 
            href.startsWith('javascript:') || 
            href.startsWith('mailto:') ||
            href.startsWith('tel:') ||
            href.startsWith('http://') ||
            href.startsWith('https://') ||
            link.hasAttribute('data-no-ajax') ||
            link.hasAttribute('target') ||
            link.hasAttribute('download')) {
            return;
        }

        // Skip logout link (needs full reload for auth)
        if (href.includes('/Logout') || href.includes('/Account/Login')) {
            return;
        }

        e.preventDefault();
        navigateTo(href);
    }, true);

    // Handle browser back/forward buttons
    window.addEventListener('popstate', function(e) {
        if (e.state && e.state.url) {
            navigateTo(e.state.url, false);
        }
    });

    async function navigateTo(url, pushState = true) {
        showPageLoader();

        try {
            const response = await fetch(url, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            const html = await response.text();
            updatePageContent(html);

            // Update URL without reload
            if (pushState) {
                history.pushState({ url: url }, '', url);
            }

            // Scroll to top
            window.scrollTo({ top: 0, behavior: 'smooth' });

            // Update active nav link
            updateActiveNavLink(url);

            // Dispatch navigation event
            document.dispatchEvent(new CustomEvent('ajax:navigate', { detail: { url } }));

        } catch (error) {
            console.error('Navigation Error:', error);
            // Fallback to traditional navigation on error
            window.location.href = url;
        } finally {
            hidePageLoader();
        }
    }

    function updatePageContent(html) {
        // Parse the HTML
        const parser = new DOMParser();
        const doc = parser.parseFromString(html, 'text/html');

        // Try to find main content area
        const newMain = doc.querySelector('main') || doc.querySelector('[data-content]') || doc.body;
        const currentMain = document.querySelector('main') || document.querySelector('[data-content]');

        if (currentMain && newMain) {
            // Fade out current content
            currentMain.style.opacity = '0';
            currentMain.style.transition = 'opacity 0.15s ease';
            
            setTimeout(() => {
                currentMain.innerHTML = newMain.innerHTML;
                currentMain.style.opacity = '1';
                
                // Re-run any inline scripts
                executeScripts(currentMain);
                
                // Update page title
                const newTitle = doc.querySelector('title');
                if (newTitle) {
                    document.title = newTitle.textContent;
                }
            }, 150);
        } else {
            // Fallback: update body content (keeping layout)
            const bodyContent = doc.body.innerHTML;
            document.body.innerHTML = bodyContent;
            executeScripts(document.body);
        }
    }

    function executeScripts(container) {
        const scripts = container.querySelectorAll('script');
        scripts.forEach(oldScript => {
            const newScript = document.createElement('script');
            Array.from(oldScript.attributes).forEach(attr => {
                newScript.setAttribute(attr.name, attr.value);
            });
            newScript.textContent = oldScript.textContent;
            oldScript.parentNode.replaceChild(newScript, oldScript);
        });
    }

    function updateActiveNavLink(url) {
        // Remove active class from all nav links
        document.querySelectorAll('.sidebar-link, .nav-link').forEach(link => {
            link.classList.remove('active', 'bg-white/15');
        });

        // Add active class to matching link
        const path = new URL(url, window.location.origin).pathname;
        document.querySelectorAll(`a[href="${path}"], a[href="${url}"]`).forEach(link => {
            link.classList.add('active');
            if (link.classList.contains('sidebar-link')) {
                link.classList.add('bg-white/15');
            }
        });
    }

    // ============================================
    // DELETE CONFIRMATION
    // ============================================

    document.addEventListener('click', async function(e) {
        const deleteBtn = e.target.closest('[data-delete], [data-ajax-delete]');
        if (!deleteBtn) return;

        e.preventDefault();
        e.stopPropagation();

        const url = deleteBtn.getAttribute('data-delete') || deleteBtn.getAttribute('data-ajax-delete');
        const confirmText = deleteBtn.getAttribute('data-confirm') || 'Are you sure you want to delete this?';
        const itemName = deleteBtn.getAttribute('data-item-name') || 'item';

        const result = await Swal.fire({
            title: 'Delete ' + itemName + '?',
            text: confirmText,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: '<i class="fas fa-trash mr-2"></i>Yes, delete',
            cancelButtonText: 'Cancel',
            confirmButtonColor: '#ef4444',
            cancelButtonColor: '#6b7280',
            ...SwalTheme.glass
        });

        if (result.isConfirmed) {
            await performDelete(url, deleteBtn);
        }
    }, true);

    async function performDelete(url, button) {
        const row = button.closest('tr, [data-item], .item-row');
        const originalHtml = button.innerHTML;
        
        button.disabled = true;
        button.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';

        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                body: token ? `__RequestVerificationToken=${encodeURIComponent(token)}` : ''
            });

            const contentType = response.headers.get('content-type') || '';
            let data = {};

            if (contentType.includes('application/json')) {
                data = await response.json();
            } else {
                data = { success: response.ok };
            }

            if (response.ok && data.success !== false) {
                showToast('success', data.message || 'Deleted successfully');

                // Animate and remove row
                if (row) {
                    row.style.transition = 'all 0.3s ease';
                    row.style.opacity = '0';
                    row.style.transform = 'translateX(-30px)';
                    row.style.height = row.offsetHeight + 'px';
                    
                    setTimeout(() => {
                        row.style.height = '0';
                        row.style.padding = '0';
                        row.style.margin = '0';
                        row.style.overflow = 'hidden';
                    }, 200);
                    
                    setTimeout(() => row.remove(), 400);
                }

                // Refresh data if callback exists
                if (typeof window.refreshPageData === 'function') {
                    setTimeout(() => window.refreshPageData(), 500);
                }

                document.dispatchEvent(new CustomEvent('ajax:deleted', { detail: data }));
            } else {
                showToast('error', data.message || 'Failed to delete');
                button.disabled = false;
                button.innerHTML = originalHtml;
            }
        } catch (error) {
            console.error('Delete Error:', error);
            showToast('error', 'An error occurred');
            button.disabled = false;
            button.innerHTML = originalHtml;
        }
    }

    // ============================================
    // UI HELPERS
    // ============================================

    function showPageLoader() {
        let loader = document.getElementById('spa-loader');
        if (!loader) {
            loader = document.createElement('div');
            loader.id = 'spa-loader';
            loader.innerHTML = `
                <div class="fixed inset-0 z-[9999] flex items-center justify-center pointer-events-none">
                    <div class="bg-black/40 backdrop-blur-sm rounded-2xl px-8 py-6 flex items-center space-x-4">
                        <div class="w-8 h-8 border-4 border-white/30 border-t-white rounded-full animate-spin"></div>
                        <span class="text-white font-medium">Loading...</span>
                    </div>
                </div>
            `;
            document.body.appendChild(loader);
        }
        loader.style.display = 'block';
        loader.style.opacity = '0';
        setTimeout(() => loader.style.opacity = '1', 10);
        loader.style.transition = 'opacity 0.2s ease';
    }

    function hidePageLoader() {
        const loader = document.getElementById('spa-loader');
        if (loader) {
            loader.style.opacity = '0';
            setTimeout(() => loader.style.display = 'none', 200);
        }
    }

    function showToast(type, message) {
        const icons = {
            success: 'fas fa-check-circle',
            error: 'fas fa-times-circle',
            warning: 'fas fa-exclamation-triangle',
            info: 'fas fa-info-circle'
        };
        
        const colors = {
            success: 'from-emerald-500/20 to-emerald-600/20 border-emerald-500/30',
            error: 'from-red-500/20 to-red-600/20 border-red-500/30',
            warning: 'from-amber-500/20 to-amber-600/20 border-amber-500/30',
            info: 'from-blue-500/20 to-blue-600/20 border-blue-500/30'
        };

        const iconColors = {
            success: 'text-emerald-400',
            error: 'text-red-400',
            warning: 'text-amber-400',
            info: 'text-blue-400'
        };

        // Remove existing toasts
        document.querySelectorAll('.spa-toast').forEach(t => t.remove());

        const toast = document.createElement('div');
        toast.className = `spa-toast fixed top-4 right-4 z-[10000] max-w-md bg-gradient-to-r ${colors[type]} backdrop-blur-xl border rounded-xl p-4 shadow-2xl transform translate-x-full transition-transform duration-300`;
        toast.innerHTML = `
            <div class="flex items-start space-x-3">
                <i class="${icons[type]} ${iconColors[type]} text-xl mt-0.5"></i>
                <div class="flex-1">
                    <p class="text-white font-medium">${message}</p>
                </div>
                <button onclick="this.closest('.spa-toast').remove()" class="text-white/60 hover:text-white">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `;
        
        document.body.appendChild(toast);
        
        // Animate in
        setTimeout(() => toast.style.transform = 'translateX(0)', 10);
        
        // Auto remove
        setTimeout(() => {
            toast.style.transform = 'translateX(120%)';
            setTimeout(() => toast.remove(), 300);
        }, 4000);
    }

    function highlightValidationErrors(form, errors) {
        // Clear previous errors
        form.querySelectorAll('.validation-error').forEach(el => el.remove());
        form.querySelectorAll('.border-red-500').forEach(el => el.classList.remove('border-red-500'));

        if (typeof errors !== 'object') return;

        Object.entries(errors).forEach(([field, messages]) => {
            const input = form.querySelector(`[name="${field}"], [name$=".${field}"]`);
            if (input) {
                input.classList.add('border-red-500');
                const errorDiv = document.createElement('div');
                errorDiv.className = 'validation-error text-red-400 text-sm mt-1';
                errorDiv.textContent = Array.isArray(messages) ? messages[0] : messages;
                input.parentNode.appendChild(errorDiv);
            }
        });
    }

    function closeModal(modal) {
        modal.style.opacity = '0';
        modal.style.transition = 'opacity 0.2s ease';
        setTimeout(() => {
            modal.style.display = 'none';
            modal.remove();
        }, 200);
    }

    // ============================================
    // GLOBAL API
    // ============================================

    // Expose navigation function globally
    window.spaNavigate = navigateTo;
    
    // Expose toast function globally
    window.showToast = showToast;

    // AJAX request helper
    window.ajaxRequest = async function(url, options = {}) {
        const defaults = {
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/json'
            }
        };

        const config = { ...defaults, ...options };
        
        if (config.body && typeof config.body === 'object' && !(config.body instanceof FormData)) {
            config.body = JSON.stringify(config.body);
        }

        showPageLoader();
        
        try {
            const response = await fetch(url, config);
            const contentType = response.headers.get('content-type') || '';
            
            hidePageLoader();
            
            if (contentType.includes('application/json')) {
                return await response.json();
            }
            
            return { success: response.ok };
        } catch (error) {
            hidePageLoader();
            throw error;
        }
    };

    // Submit form programmatically
    window.submitForm = function(form) {
        if (typeof form === 'string') {
            form = document.querySelector(form);
        }
        if (form) {
            handleFormSubmit(form);
        }
    };

    // Refresh current page content without reload
    window.refreshPage = async function() {
        await navigateTo(window.location.href, false);
    };

    // Initialize history state
    history.replaceState({ url: window.location.href }, '', window.location.href);

    console.log('🚀 SPA Mode Active - Zero Page Reloads');

})();
