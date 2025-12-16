/**
 * Mess Management System - Validation & UI Enhancement Library
 * Professional client-side validation, toast notifications, and confirmation dialogs
 * Styled to match the landing page and login page design
 */

// ============================================
// SWEETALERT2 CUSTOM THEME - GLASS MORPHISM
// ============================================

const SwalTheme = {
    // Base glass morphism styling matching landing/login pages
    glass: {
        background: 'linear-gradient(135deg, rgba(255, 255, 255, 0.15) 0%, rgba(255, 255, 255, 0.05) 100%)',
        backdrop: `
            rgba(0, 0, 0, 0.6)
            url("https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=1920&h=1080&fit=crop")
            center/cover
            no-repeat
        `,
        color: '#fff',
        customClass: {
            popup: 'swal-glass-popup',
            title: 'swal-glass-title',
            htmlContainer: 'swal-glass-content',
            confirmButton: 'swal-btn-primary',
            cancelButton: 'swal-btn-secondary',
            denyButton: 'swal-btn-danger',
            icon: 'swal-glass-icon'
        }
    }
};

// Inject custom styles for SweetAlert2
(function() {
    const style = document.createElement('style');
    style.textContent = `
        .swal-glass-popup {
            background: linear-gradient(135deg, rgba(255, 255, 255, 0.18) 0%, rgba(255, 255, 255, 0.08) 100%) !important;
            backdrop-filter: blur(20px) !important;
            -webkit-backdrop-filter: blur(20px) !important;
            border: 1px solid rgba(255, 255, 255, 0.25) !important;
            border-radius: 24px !important;
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5), 0 0 0 1px rgba(255, 255, 255, 0.1) inset !important;
        }
        
        .swal-glass-title {
            color: #fff !important;
            font-family: 'Inter', sans-serif !important;
            font-weight: 700 !important;
            font-size: 1.5rem !important;
        }
        
        .swal-glass-content {
            color: rgba(255, 255, 255, 0.8) !important;
            font-family: 'Inter', sans-serif !important;
        }
        
        .swal-btn-primary {
            background: linear-gradient(135deg, #059669 0%, #10b981 100%) !important;
            border: none !important;
            border-radius: 12px !important;
            padding: 12px 28px !important;
            font-weight: 600 !important;
            font-family: 'Inter', sans-serif !important;
            transition: all 0.3s ease !important;
            box-shadow: 0 4px 15px rgba(16, 185, 129, 0.4) !important;
        }
        
        .swal-btn-primary:hover {
            transform: translateY(-2px) !important;
            box-shadow: 0 8px 25px rgba(16, 185, 129, 0.5) !important;
        }
        
        .swal-btn-secondary {
            background: rgba(255, 255, 255, 0.15) !important;
            backdrop-filter: blur(10px) !important;
            border: 1px solid rgba(255, 255, 255, 0.25) !important;
            border-radius: 12px !important;
            padding: 12px 28px !important;
            font-weight: 600 !important;
            font-family: 'Inter', sans-serif !important;
            color: #fff !important;
            transition: all 0.3s ease !important;
        }
        
        .swal-btn-secondary:hover {
            background: rgba(255, 255, 255, 0.25) !important;
            transform: translateY(-2px) !important;
        }
        
        .swal-btn-danger {
            background: linear-gradient(135deg, #dc2626 0%, #ef4444 100%) !important;
            border: none !important;
            border-radius: 12px !important;
            padding: 12px 28px !important;
            font-weight: 600 !important;
            font-family: 'Inter', sans-serif !important;
            transition: all 0.3s ease !important;
            box-shadow: 0 4px 15px rgba(239, 68, 68, 0.4) !important;
        }
        
        .swal-btn-danger:hover {
            transform: translateY(-2px) !important;
            box-shadow: 0 8px 25px rgba(239, 68, 68, 0.5) !important;
        }
        
        .swal-glass-icon {
            border-color: rgba(255, 255, 255, 0.3) !important;
        }
        
        .swal-glass-icon.swal2-success {
            border-color: rgba(16, 185, 129, 0.5) !important;
        }
        
        .swal-glass-icon.swal2-success [class^=swal2-success-line] {
            background-color: #10b981 !important;
        }
        
        .swal-glass-icon.swal2-success .swal2-success-ring {
            border-color: rgba(16, 185, 129, 0.3) !important;
        }
        
        .swal-glass-icon.swal2-error {
            border-color: rgba(239, 68, 68, 0.5) !important;
        }
        
        .swal-glass-icon.swal2-error [class^=swal2-x-mark-line] {
            background-color: #ef4444 !important;
        }
        
        .swal-glass-icon.swal2-warning {
            border-color: rgba(245, 158, 11, 0.5) !important;
            color: #f59e0b !important;
        }
        
        .swal-glass-icon.swal2-info {
            border-color: rgba(59, 130, 246, 0.5) !important;
            color: #3b82f6 !important;
        }
        
        .swal-glass-icon.swal2-question {
            border-color: rgba(139, 92, 246, 0.5) !important;
            color: #8b5cf6 !important;
        }
        
        /* Toast styling */
        .swal-toast-glass {
            background: linear-gradient(135deg, rgba(255, 255, 255, 0.2) 0%, rgba(255, 255, 255, 0.1) 100%) !important;
            backdrop-filter: blur(15px) !important;
            -webkit-backdrop-filter: blur(15px) !important;
            border: 1px solid rgba(255, 255, 255, 0.2) !important;
            border-radius: 16px !important;
            box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3) !important;
        }
        
        .swal-toast-success {
            background: linear-gradient(135deg, rgba(16, 185, 129, 0.9) 0%, rgba(5, 150, 105, 0.9) 100%) !important;
        }
        
        .swal-toast-error {
            background: linear-gradient(135deg, rgba(239, 68, 68, 0.9) 0%, rgba(220, 38, 38, 0.9) 100%) !important;
        }
        
        .swal-toast-warning {
            background: linear-gradient(135deg, rgba(245, 158, 11, 0.9) 0%, rgba(217, 119, 6, 0.9) 100%) !important;
        }
        
        .swal-toast-info {
            background: linear-gradient(135deg, rgba(59, 130, 246, 0.9) 0%, rgba(37, 99, 235, 0.9) 100%) !important;
        }
        
        /* Logout popup styling - matching landing page */
        .swal-logout-popup {
            background: linear-gradient(135deg, rgba(255, 255, 255, 0.2) 0%, rgba(255, 255, 255, 0.08) 100%) !important;
            backdrop-filter: blur(25px) !important;
            -webkit-backdrop-filter: blur(25px) !important;
            border: 1px solid rgba(255, 255, 255, 0.3) !important;
            border-radius: 28px !important;
            box-shadow: 0 30px 60px -15px rgba(0, 0, 0, 0.6), 0 0 0 1px rgba(255, 255, 255, 0.15) inset !important;
            padding: 20px !important;
        }
        
        .swal-btn-logout {
            background: linear-gradient(135deg, #dc2626 0%, #ef4444 100%) !important;
            border: none !important;
            border-radius: 14px !important;
            padding: 14px 32px !important;
            font-weight: 600 !important;
            font-size: 1rem !important;
            font-family: 'Inter', sans-serif !important;
            transition: all 0.3s ease !important;
            box-shadow: 0 6px 20px rgba(239, 68, 68, 0.4) !important;
        }
        
        .swal-btn-logout:hover {
            transform: translateY(-3px) !important;
            box-shadow: 0 10px 30px rgba(239, 68, 68, 0.5) !important;
        }
        
        .swal-btn-stay {
            background: linear-gradient(135deg, #059669 0%, #10b981 100%) !important;
            border: none !important;
            border-radius: 14px !important;
            padding: 14px 32px !important;
            font-weight: 600 !important;
            font-size: 1rem !important;
            font-family: 'Inter', sans-serif !important;
            color: #fff !important;
            transition: all 0.3s ease !important;
            box-shadow: 0 6px 20px rgba(16, 185, 129, 0.4) !important;
        }
        
        .swal-btn-stay:hover {
            transform: translateY(-3px) !important;
            box-shadow: 0 10px 30px rgba(16, 185, 129, 0.5) !important;
        }
        
        .swal-close-btn {
            color: rgba(255, 255, 255, 0.6) !important;
            transition: all 0.2s ease !important;
        }
        
        .swal-close-btn:hover {
            color: rgba(255, 255, 255, 1) !important;
            transform: rotate(90deg) !important;
        }
        
        /* Input styling in modals */
        .swal2-input, .swal2-textarea, .swal2-select {
            background: rgba(255, 255, 255, 0.1) !important;
            border: 1px solid rgba(255, 255, 255, 0.2) !important;
            border-radius: 12px !important;
            color: #fff !important;
            font-family: 'Inter', sans-serif !important;
        }
        
        .swal2-input:focus, .swal2-textarea:focus, .swal2-select:focus {
            border-color: rgba(16, 185, 129, 0.5) !important;
            box-shadow: 0 0 20px rgba(16, 185, 129, 0.2) !important;
            background: rgba(255, 255, 255, 0.15) !important;
        }
        
        .swal2-input::placeholder {
            color: rgba(255, 255, 255, 0.5) !important;
        }
        
        .swal2-validation-message {
            background: rgba(239, 68, 68, 0.2) !important;
            color: #fca5a5 !important;
            border-radius: 8px !important;
        }
    `;
    document.head.appendChild(style);
})();

// ============================================
// TOAST NOTIFICATION SYSTEM
// ============================================

const Toast = {
    success: function(message, title = 'Success') {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'success',
            title: message,
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true,
            background: 'transparent',
            color: '#fff',
            customClass: {
                popup: 'swal-toast-glass swal-toast-success',
                timerProgressBar: 'bg-white/30'
            }
        });
    },
    
    error: function(message, title = 'Error') {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'error',
            title: message,
            showConfirmButton: false,
            timer: 4000,
            timerProgressBar: true,
            background: 'transparent',
            color: '#fff',
            customClass: {
                popup: 'swal-toast-glass swal-toast-error',
                timerProgressBar: 'bg-white/30'
            }
        });
    },
    
    warning: function(message, title = 'Warning') {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'warning',
            title: message,
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true,
            background: 'transparent',
            color: '#fff',
            customClass: {
                popup: 'swal-toast-glass swal-toast-warning',
                timerProgressBar: 'bg-white/30'
            }
        });
    },
    
    info: function(message, title = 'Info') {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'info',
            title: message,
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true,
            background: 'transparent',
            color: '#fff',
            customClass: {
                popup: 'swal-toast-glass swal-toast-info',
                timerProgressBar: 'bg-white/30'
            }
        });
    }
};

// ============================================
// CONFIRMATION DIALOGS - GLASS MORPHISM STYLE
// ============================================

const Confirm = {
    delete: function(itemName = 'item') {
        return Swal.fire({
            title: '<i class="fas fa-trash-alt text-red-400 mr-2"></i>Delete ' + itemName + '?',
            html: '<p class="text-white/70">This action cannot be undone. Are you sure you want to permanently delete this ' + itemName.toLowerCase() + '?</p>',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: '<i class="fas fa-trash mr-2"></i>Yes, Delete',
            cancelButtonText: '<i class="fas fa-times mr-2"></i>Cancel',
            reverseButtons: true,
            focusCancel: true,
            backdrop: `rgba(0, 0, 0, 0.7)`,
            ...SwalTheme.glass,
            customClass: {
                ...SwalTheme.glass.customClass,
                confirmButton: 'swal-btn-danger',
            }
        });
    },
    
    action: function(title, text, confirmText = 'Confirm', icon = 'question') {
        return Swal.fire({
            title: title,
            html: '<p class="text-white/70">' + text + '</p>',
            icon: icon,
            showCancelButton: true,
            confirmButtonText: '<i class="fas fa-check mr-2"></i>' + confirmText,
            cancelButtonText: '<i class="fas fa-times mr-2"></i>Cancel',
            reverseButtons: true,
            backdrop: `rgba(0, 0, 0, 0.7)`,
            ...SwalTheme.glass
        });
    },
    
    danger: function(title, text, confirmText = 'Proceed') {
        return Swal.fire({
            title: '<i class="fas fa-exclamation-triangle text-amber-400 mr-2"></i>' + title,
            html: '<p class="text-white/70">' + text + '</p>',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: '<i class="fas fa-exclamation-triangle mr-2"></i>' + confirmText,
            cancelButtonText: '<i class="fas fa-times mr-2"></i>Cancel',
            reverseButtons: true,
            focusCancel: true,
            backdrop: `rgba(0, 0, 0, 0.7)`,
            ...SwalTheme.glass,
            customClass: {
                ...SwalTheme.glass.customClass,
                confirmButton: 'swal-btn-danger',
            }
        });
    },
    
    logout: function() {
        return Swal.fire({
            title: 'Sign Out',
            html: `
                <div class="text-center py-4">
                    <div class="w-20 h-20 mx-auto mb-4 bg-gradient-to-br from-purple-500 to-indigo-600 rounded-full flex items-center justify-center shadow-lg">
                        <i class="fas fa-sign-out-alt text-white text-3xl"></i>
                    </div>
                    <p class="text-white/80 text-lg mb-2">Are you sure you want to sign out?</p>
                    <p class="text-white/50 text-sm">You'll need to login again to access your account</p>
                </div>
            `,
            showCancelButton: true,
            confirmButtonText: '<i class="fas fa-sign-out-alt mr-2"></i>Yes, Sign Out',
            cancelButtonText: '<i class="fas fa-arrow-left mr-2"></i>Stay Logged In',
            reverseButtons: true,
            showCloseButton: true,
            backdrop: `
                linear-gradient(rgba(0, 0, 0, 0.7), rgba(0, 0, 0, 0.7)),
                url("https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=1920&h=1080&fit=crop")
                center/cover
                no-repeat
            `,
            background: 'linear-gradient(135deg, rgba(255, 255, 255, 0.2) 0%, rgba(255, 255, 255, 0.08) 100%)',
            color: '#fff',
            customClass: {
                popup: 'swal-logout-popup',
                title: 'swal-glass-title',
                htmlContainer: 'swal-glass-content',
                confirmButton: 'swal-btn-logout',
                cancelButton: 'swal-btn-stay',
                closeButton: 'swal-close-btn'
            }
        });
    },
    
    // New: Success confirmation with animation
    success: function(title, text) {
        return Swal.fire({
            title: '<i class="fas fa-check-circle text-emerald-400 mr-2"></i>' + title,
            html: '<p class="text-white/70">' + text + '</p>',
            icon: 'success',
            confirmButtonText: '<i class="fas fa-thumbs-up mr-2"></i>Great!',
            backdrop: `rgba(0, 0, 0, 0.7)`,
            ...SwalTheme.glass
        });
    },
    
    // New: Info modal
    info: function(title, text) {
        return Swal.fire({
            title: '<i class="fas fa-info-circle text-blue-400 mr-2"></i>' + title,
            html: '<p class="text-white/70">' + text + '</p>',
            icon: 'info',
            confirmButtonText: '<i class="fas fa-check mr-2"></i>Got it',
            backdrop: `rgba(0, 0, 0, 0.7)`,
            ...SwalTheme.glass
        });
    }
};

// ============================================
// FORM VALIDATION
// ============================================

const Validator = {
    // Add validation styling to a field
    showError: function(input, message) {
        const formGroup = input.closest('.form-group') || input.parentElement;
        input.classList.add('border-red-500', 'bg-red-500/10');
        input.classList.remove('border-white/20', 'border-emerald-500');
        
        // Remove existing error message if any
        const existingError = formGroup.querySelector('.validation-error');
        if (existingError) existingError.remove();
        
        // Add error message
        const errorDiv = document.createElement('div');
        errorDiv.className = 'validation-error text-red-400 text-sm mt-1 flex items-center';
        errorDiv.innerHTML = '<i class="fas fa-exclamation-circle mr-1"></i>' + message;
        formGroup.appendChild(errorDiv);
    },
    
    // Remove validation styling from a field
    clearError: function(input) {
        const formGroup = input.closest('.form-group') || input.parentElement;
        input.classList.remove('border-red-500', 'bg-red-500/10');
        input.classList.add('border-white/20');
        
        const existingError = formGroup.querySelector('.validation-error');
        if (existingError) existingError.remove();
    },
    
    // Show success styling
    showSuccess: function(input) {
        input.classList.remove('border-red-500', 'bg-red-500/10', 'border-white/20');
        input.classList.add('border-emerald-500', 'bg-emerald-500/10');
    },
    
    // Validate required field
    required: function(input, fieldName = 'This field') {
        const value = input.value.trim();
        if (!value) {
            this.showError(input, fieldName + ' is required');
            return false;
        }
        this.clearError(input);
        return true;
    },
    
    // Validate email
    email: function(input) {
        const value = input.value.trim();
        if (!value) return true; // Skip if empty (use required for that)
        
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(value)) {
            this.showError(input, 'Please enter a valid email address');
            return false;
        }
        this.clearError(input);
        return true;
    },
    
    // Validate minimum length
    minLength: function(input, min, fieldName = 'This field') {
        const value = input.value.trim();
        if (value && value.length < min) {
            this.showError(input, fieldName + ' must be at least ' + min + ' characters');
            return false;
        }
        this.clearError(input);
        return true;
    },
    
    // Validate maximum length
    maxLength: function(input, max, fieldName = 'This field') {
        const value = input.value.trim();
        if (value && value.length > max) {
            this.showError(input, fieldName + ' must not exceed ' + max + ' characters');
            return false;
        }
        this.clearError(input);
        return true;
    },
    
    // Validate number range
    range: function(input, min, max, fieldName = 'Value') {
        const value = parseFloat(input.value);
        if (isNaN(value)) {
            this.showError(input, fieldName + ' must be a valid number');
            return false;
        }
        if (value < min || value > max) {
            this.showError(input, fieldName + ' must be between ' + min + ' and ' + max);
            return false;
        }
        this.clearError(input);
        return true;
    },
    
    // Validate minimum number
    min: function(input, min, fieldName = 'Value') {
        const value = parseFloat(input.value);
        if (isNaN(value) || value < min) {
            this.showError(input, fieldName + ' must be at least ' + min);
            return false;
        }
        this.clearError(input);
        return true;
    },
    
    // Validate password match
    passwordMatch: function(password, confirmPassword) {
        if (password.value !== confirmPassword.value) {
            this.showError(confirmPassword, 'Passwords do not match');
            return false;
        }
        this.clearError(confirmPassword);
        return true;
    },
    
    // Validate password strength
    passwordStrength: function(input) {
        const value = input.value;
        if (!value) return true;
        
        const hasLetter = /[a-zA-Z]/.test(value);
        const hasNumber = /[0-9]/.test(value);
        
        if (value.length < 6) {
            this.showError(input, 'Password must be at least 6 characters');
            return false;
        }
        
        this.clearError(input);
        return true;
    }
};

// ============================================
// FORM ENHANCEMENT
// ============================================

const FormHelper = {
    // Initialize form with validation and loading states
    init: function(formSelector, options = {}) {
        const form = document.querySelector(formSelector);
        if (!form) return;
        
        const submitBtn = form.querySelector('button[type="submit"]');
        const originalBtnContent = submitBtn ? submitBtn.innerHTML : '';
        
        // Add loading state on submit
        form.addEventListener('submit', function(e) {
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin mr-2"></i>Processing...';
                submitBtn.classList.add('opacity-75', 'cursor-not-allowed');
            }
            
            // Re-enable after 10 seconds as fallback
            setTimeout(function() {
                if (submitBtn) {
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = originalBtnContent;
                    submitBtn.classList.remove('opacity-75', 'cursor-not-allowed');
                }
            }, 10000);
        });
        
        // Real-time validation on blur
        form.querySelectorAll('input, select, textarea').forEach(function(input) {
            input.addEventListener('blur', function() {
                if (input.hasAttribute('required') && !input.value.trim()) {
                    const label = input.closest('.form-group')?.querySelector('label')?.textContent || 'This field';
                    Validator.showError(input, label.replace('*', '').trim() + ' is required');
                } else {
                    Validator.clearError(input);
                }
            });
            
            // Clear error on input
            input.addEventListener('input', function() {
                Validator.clearError(input);
            });
        });
    },
    
    // Disable form during submission
    setLoading: function(form, isLoading) {
        const submitBtn = form.querySelector('button[type="submit"]');
        const inputs = form.querySelectorAll('input, select, textarea');
        
        if (isLoading) {
            if (submitBtn) {
                submitBtn.dataset.originalContent = submitBtn.innerHTML;
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin mr-2"></i>Processing...';
                submitBtn.classList.add('opacity-75', 'cursor-not-allowed');
            }
            inputs.forEach(i => i.disabled = true);
        } else {
            if (submitBtn) {
                submitBtn.disabled = false;
                submitBtn.innerHTML = submitBtn.dataset.originalContent || 'Submit';
                submitBtn.classList.remove('opacity-75', 'cursor-not-allowed');
            }
            inputs.forEach(i => i.disabled = false);
        }
    }
};

// ============================================
// CONFIRMATION HANDLERS
// ============================================

// Handle delete confirmation with form submission
function confirmDelete(formId, itemName = 'item') {
    Confirm.delete(itemName).then((result) => {
        if (result.isConfirmed) {
            document.getElementById(formId).submit();
        }
    });
    return false;
}

// Handle any action confirmation with form submission
function confirmAction(formId, title, text, confirmText = 'Confirm') {
    Confirm.action(title, text, confirmText).then((result) => {
        if (result.isConfirmed) {
            document.getElementById(formId).submit();
        }
    });
    return false;
}

// Handle status change confirmation
function confirmStatusChange(formId, newStatus) {
    const title = 'Change Status?';
    const text = 'Change status to ' + newStatus + '?';
    Confirm.action(title, text, 'Change Status').then((result) => {
        if (result.isConfirmed) {
            document.getElementById(formId).submit();
        }
    });
    return false;
}

// Handle logout confirmation
function confirmLogout(logoutUrl) {
    Confirm.logout().then((result) => {
        if (result.isConfirmed) {
            window.location.href = logoutUrl;
        }
    });
    return false;
}

// ============================================
// UTILITY FUNCTIONS
// ============================================

// Format currency
function formatCurrency(amount, currency = 'Rs.') {
    return currency + parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

// Debounce function for search inputs
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// ============================================
// AUTO-INITIALIZATION
// ============================================

document.addEventListener('DOMContentLoaded', function() {
    // Add input-glass styling feedback on focus/blur
    document.querySelectorAll('.input-glass, input[type="text"], input[type="password"], input[type="email"], input[type="number"], select, textarea').forEach(function(input) {
        if (!input.classList.contains('no-validation')) {
            input.addEventListener('focus', function() {
                this.classList.add('ring-2', 'ring-emerald-500/50');
            });
            
            input.addEventListener('blur', function() {
                this.classList.remove('ring-2', 'ring-emerald-500/50');
            });
        }
    });
    
    // Handle all logout links
    document.querySelectorAll('a[href*="/Account/Logout"]').forEach(function(link) {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            confirmLogout(this.href);
        });
    });
});

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { Toast, Confirm, Validator, FormHelper };
}
