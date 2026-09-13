/* ============================================================
   Ahmed OS — Core JavaScript
   ============================================================ */

(function () {
    'use strict';

    // ============================================================
    // THEME MANAGEMENT
    // ============================================================
    const ThemeManager = {
        init() {
            const saved = localStorage.getItem('ahmedos-theme') || 'dark';
            this.setTheme(saved);
            const toggle = document.getElementById('themeToggle');
            if (toggle) {
                toggle.addEventListener('click', () => {
                    const current = document.documentElement.getAttribute('data-theme');
                    this.setTheme(current === 'dark' ? 'light' : 'dark');
                });
            }
        },
        setTheme(theme) {
            document.documentElement.setAttribute('data-theme', theme);
            localStorage.setItem('ahmedos-theme', theme);
            const toggle = document.getElementById('themeToggle');
            if (toggle) {
                toggle.textContent = theme === 'dark' ? '🌙' : '☀️';
            }
        }
    };

    // ============================================================
    // SIDEBAR / MOBILE NAV
    // ============================================================
    const Sidebar = {
        init() {
            const toggle = document.getElementById('menuToggle');
            const sidebar = document.getElementById('sidebar');
            if (toggle && sidebar) {
                toggle.addEventListener('click', () => {
                    sidebar.classList.toggle('open');
                });
                // Close on outside click (mobile)
                document.addEventListener('click', (e) => {
                    if (sidebar.classList.contains('open') && !sidebar.contains(e.target) && !toggle.contains(e.target)) {
                        sidebar.classList.remove('open');
                    }
                });
            }
        }
    };

    // ============================================================
    // COMMAND PALETTE
    // ============================================================
    const CommandPalette = {
        init() {
            this.el = document.getElementById('commandPalette');
            this.input = document.getElementById('commandInput');
            this.results = document.getElementById('commandResults');
            this.searchTrigger = document.getElementById('searchTrigger');

            if (!this.el) return;

            // Keyboard shortcut
            document.addEventListener('keydown', (e) => {
                if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
                    e.preventDefault();
                    this.toggle();
                }
                if (e.key === 'Escape' && this.el.classList.contains('active')) {
                    this.close();
                }
            });

            if (this.searchTrigger) {
                this.searchTrigger.addEventListener('click', () => this.open());
            }

            // Close on backdrop click
            this.el.addEventListener('click', (e) => {
                if (e.target === this.el) this.close();
            });

            // Filter commands
            if (this.input) {
                this.input.addEventListener('input', () => this.filter());
            }

            // Handle command clicks
            if (this.results) {
                this.results.addEventListener('click', (e) => {
                    const item = e.target.closest('.command-item');
                    if (item) {
                        this.executeCommand(item.dataset.action);
                        this.close();
                    }
                });
            }
        },
        open() {
            this.el.classList.add('active');
            setTimeout(() => this.input?.focus(), 50);
        },
        close() {
            this.el.classList.remove('active');
            if (this.input) this.input.value = '';
            this.filter();
        },
        toggle() {
            if (this.el.classList.contains('active')) this.close();
            else this.open();
        },
        filter() {
            const query = this.input?.value?.toLowerCase() || '';
            const items = this.results?.querySelectorAll('.command-item') || [];
            items.forEach(item => {
                const text = item.textContent.toLowerCase();
                item.style.display = text.includes(query) ? '' : 'none';
            });
        },
        executeCommand(action) {
            const routes = {
                newTask: '/Tasks?action=create',
                newNote: '/Notes?action=create',
                newProject: '/Projects?action=create',
                newGoal: '/Goals?action=create',
                newHabit: '/Habits?action=create',
                startFocus: '/Focus',
                openToday: '/Today',
                openCalendar: '/Calendar',
                planDay: '/Planner',
                reviewDay: '/Reviews',
            };
            if (routes[action]) {
                window.location.href = routes[action];
            }
        }
    };

    // ============================================================
    // QUICK CAPTURE
    // ============================================================
    const QuickCapture = {
        init() {
            const btn = document.getElementById('quickCaptureBtn');
            const fab = document.getElementById('fabBtn');

            if (btn) btn.addEventListener('click', () => this.show());
            if (fab) fab.addEventListener('click', () => this.show());

            // Keyboard shortcut
            document.addEventListener('keydown', (e) => {
                if (e.ctrlKey && e.shiftKey && e.key === 'A') {
                    e.preventDefault();
                    this.show();
                }
            });
        },
        show() {
            // For now, navigate to inbox with create mode
            window.location.href = '/Inbox?action=capture';
        }
    };

    // ============================================================
    // TASK CHECKBOX
    // ============================================================
    const TaskActions = {
        init() {
            document.addEventListener('click', (e) => {
                const checkbox = e.target.closest('.task-checkbox:not(.checked)');
                if (checkbox && checkbox.dataset.taskId) {
                    this.completeTask(checkbox.dataset.taskId, checkbox);
                }
            });
        },
        async completeTask(taskId, checkbox) {
            checkbox.classList.add('checked');
            const titleEl = checkbox.closest('.task-item')?.querySelector('.task-title');
            if (titleEl) titleEl.classList.add('completed');

            try {
                const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
                const response = await fetch(`/api/tasks/${taskId}/complete`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token || ''
                    }
                });
                if (response.ok) {
                    Toast.show('Task completed ✓', 'success');
                }
            } catch (err) {
                console.error('Failed to complete task:', err);
                checkbox.classList.remove('checked');
                if (titleEl) titleEl.classList.remove('completed');
            }
        }
    };

    // ============================================================
    // TOAST NOTIFICATIONS
    // ============================================================
    const Toast = {
        show(message, type = 'info', duration = 3000) {
            const container = document.getElementById('toastContainer');
            if (!container) return;

            const toast = document.createElement('div');
            toast.className = `toast toast-${type}`;
            toast.textContent = message;
            container.appendChild(toast);

            setTimeout(() => {
                toast.style.opacity = '0';
                toast.style.transform = 'translateX(20px)';
                toast.style.transition = 'all 200ms ease';
                setTimeout(() => toast.remove(), 200);
            }, duration);
        }
    };

    // Make Toast globally available
    window.AhmedOS = window.AhmedOS || {};
    window.AhmedOS.Toast = Toast;

    // ============================================================
    // GREETING
    // ============================================================
    const Greeting = {
        init() {
            const el = document.getElementById('greetingText');
            if (!el) return;
            const hour = new Date().getHours();
            let greeting = 'Good evening';
            if (hour < 12) greeting = 'Good morning';
            else if (hour < 17) greeting = 'Good afternoon';
            el.textContent = `${greeting}, Ahmed`;
        }
    };

    // ============================================================
    // TIME DISPLAY
    // ============================================================
    const Clock = {
        init() {
            this.update();
            setInterval(() => this.update(), 60000);
        },
        update() {
            const el = document.getElementById('currentTime');
            if (!el) return;
            const now = new Date();
            const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
            el.textContent = now.toLocaleDateString('en-US', options);
        }
    };

    // ============================================================
    // HABIT TOGGLE
    // ============================================================
    const HabitToggle = {
        init() {
            document.addEventListener('click', (e) => {
                const check = e.target.closest('.habit-check');
                if (check && check.dataset.habitId) {
                    check.classList.toggle('done');
                    if (check.classList.contains('done')) {
                        Toast.show('Habit completed ✓', 'success');
                    }
                }
            });
        }
    };

    // ============================================================
    // TAB SWITCHING
    // ============================================================
    const Tabs = {
        init() {
            document.addEventListener('click', (e) => {
                const tab = e.target.closest('.tab');
                if (!tab) return;
                const tabGroup = tab.closest('.tabs');
                if (!tabGroup) return;
                tabGroup.querySelectorAll('.tab').forEach(t => t.classList.remove('active'));
                tab.classList.add('active');

                const target = tab.dataset.tab;
                if (target) {
                    const panel = document.querySelector(`.tab-panel[data-tab="${target}"]`);
                    if (panel) {
                        document.querySelectorAll('.tab-panel').forEach(p => p.style.display = 'none');
                        panel.style.display = '';
                    }
                }
            });
        }
    };

    // ============================================================
    // BADGES UPDATE
    // ============================================================
    const Badges = {
        init() {
            const todayBadge = document.getElementById('todayBadge');
            const inboxBadge = document.getElementById('inboxBadge');
            // These will be populated from the API later
            if (todayBadge) todayBadge.style.display = 'none';
            if (inboxBadge) inboxBadge.style.display = 'none';
        }
    };

    // ============================================================
    // INITIALIZE
    // ============================================================
    document.addEventListener('DOMContentLoaded', () => {
        ThemeManager.init();
        Sidebar.init();
        CommandPalette.init();
        QuickCapture.init();
        TaskActions.init();
        Greeting.init();
        Clock.init();
        HabitToggle.init();
        Tabs.init();
        Badges.init();
    });
})();
