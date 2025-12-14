/**
 * Mess Management System - Validation & UI Enhancement Library
 * Professional client-side validation, toast notifications, and confirmation dialogs
 */

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
            background: 'rgba(16, 185, 129, 0.95)',
            color: '#fff',
            customClass: {
                popup: 'rounded-xl shadow-2xl'
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
            background: 'rgba(239, 68, 68, 0.95)',
            color: '#fff',
            customClass: {
                popup: 'rounded-xl shadow-2xl'
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
            background: 'rgba(245, 158, 11, 0.95)',
            color: '#fff',
            customClass: {
                popup: 'rounded-xl shadow-2xl'
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
            background: 'rgba(59, 130, 246, 0.95)',
            color: '#fff',
            customClass: {
                popup: 'rounded-xl shadow-2xl'
            }
        });
    }
};

// ============================================
// CONFIRMATION DIALOGS
// ============================================

const Confirm = {
    delete: function(itemName = 'item') {
        return Swal.fire({
            title: 'Delete ' + itemName + '?',
            text: "This action cannot be undone!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#ef4444',
            cancelButtonColor: '#6b7280',
            confirmButtonText: '<i class="fas fa-trash mr-2"></i>Yes, delete it!',
            cancelButtonText: '<i class="fas fa-times mr-2"></i>Cancel',
            background: 'rgba(17, 24, 39, 0.95)',
            color: '#fff',
            backdrop: 'rgba(0,0,0,0.7)',
            customClass: {
                popup: 'rounded-2xl border border-white/20',
                confirmButton: 'rounded-xl px-6 py-2',
                cancelButton: 'rounded-xl px-6 py-2'
            }
        });
    },
    
    action: function(title, text, confirmText = 'Confirm', icon = 'question') {
        return Swal.fire({
            title: title,
            text: text,
            icon: icon,
            showCancelButton: true,
            confirmButtonColor: '#10b981',
            cancelButtonColor: '#6b7280',
            confirmButtonText: '<i class="fas fa-check mr-2"></i>' + confirmText,
            cancelButtonText: '<i class="fas fa-times mr-2"></i>Cancel',
            background: 'rgba(17, 24, 39, 0.95)',
            color: '#fff',
            backdrop: 'rgba(0,0,0,0.7)',
            customClass: {
                popup: 'rounded-2xl border border-white/20',
                confirmButton: 'rounded-xl px-6 py-2',
                cancelButton: 'rounded-xl px-6 py-2'
            }
        });
    },
    
    danger: function(title, text, confirmText = 'Proceed') {
        return Swal.fire({
            title: title,
            text: text,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#ef4444',
            cancelButtonColor: '#6b7280',
            confirmButtonText: '<i class="fas fa-exclamation-triangle mr-2"></i>' + confirmText,
            cancelButtonText: '<i class="fas fa-times mr-2"></i>Cancel',
            background: 'rgba(17, 24, 39, 0.95)',
            color: '#fff',
            backdrop: 'rgba(0,0,0,0.7)',
            customClass: {
                popup: 'rounded-2xl border border-white/20',
                confirmButton: 'rounded-xl px-6 py-2',
                cancelButton: 'rounded-xl px-6 py-2'
            }
        });
    },
    
    logout: function() {
        return Swal.fire({
            title: 'Logout?',
            text: "Are you sure you want to sign out?",
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#ef4444',
            cancelButtonColor: '#6b7280',
            confirmButtonText: '<i class="fas fa-sign-out-alt mr-2"></i>Yes, logout',
            cancelButtonText: '<i class="fas fa-times mr-2"></i>Stay',
            background: 'rgba(17, 24, 39, 0.95)',
            color: '#fff',
            backdrop: 'rgba(0,0,0,0.7)',
            customClass: {
                popup: 'rounded-2xl border border-white/20',
                confirmButton: 'rounded-xl px-6 py-2',
                cancelButton: 'rounded-xl px-6 py-2'
            }
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
