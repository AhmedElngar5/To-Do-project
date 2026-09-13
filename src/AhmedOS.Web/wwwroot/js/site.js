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
    // COMMAND PALETTE & GLOBAL SEARCH
    // ============================================================
    const CommandPalette = {
        debounceTimer: null,
        defaultCommands: '',

        init() {
            this.el = document.getElementById('commandPalette');
            this.input = document.getElementById('commandInput');
            this.results = document.getElementById('commandResults');
            this.searchTrigger = document.getElementById('searchTrigger');

            if (!this.el) return;

            // Cache original static commands
            if (this.results) {
                this.defaultCommands = this.results.innerHTML;
            }

            // Keyboard shortcut Ctrl+K / Cmd+K
            document.addEventListener('keydown', (e) => {
                if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') {
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

            // Arrow key navigation & Enter execution
            if (this.input) {
                this.input.addEventListener('keydown', (e) => {
                    const visibleItems = Array.from(this.results?.querySelectorAll('.command-item') || []).filter(item => item.style.display !== 'none');
                    if (!visibleItems.length) return;

                    const currentIndex = visibleItems.findIndex(item => item.classList.contains('selected'));

                    if (e.key === 'ArrowDown') {
                        e.preventDefault();
                        const nextIndex = currentIndex < visibleItems.length - 1 ? currentIndex + 1 : 0;
                        visibleItems.forEach((item, idx) => item.classList.toggle('selected', idx === nextIndex));
                        visibleItems[nextIndex]?.scrollIntoView({ block: 'nearest' });
                    } else if (e.key === 'ArrowUp') {
                        e.preventDefault();
                        const prevIndex = currentIndex > 0 ? currentIndex - 1 : visibleItems.length - 1;
                        visibleItems.forEach((item, idx) => item.classList.toggle('selected', idx === prevIndex));
                        visibleItems[prevIndex]?.scrollIntoView({ block: 'nearest' });
                    } else if (e.key === 'Enter') {
                        e.preventDefault();
                        const target = currentIndex >= 0 ? visibleItems[currentIndex] : visibleItems[0];
                        if (target) {
                            if (target.dataset.url) {
                                window.location.href = target.dataset.url;
                            } else if (target.dataset.action) {
                                this.executeCommand(target.dataset.action);
                            }
                            this.close();
                        }
                    }
                });

                // Filter commands & search API
                this.input.addEventListener('input', () => {
                    const query = this.input.value.trim();
                    this.filterStatic(query);

                    clearTimeout(this.debounceTimer);
                    if (query.length >= 2) {
                        this.debounceTimer = setTimeout(() => this.searchApi(query), 200);
                    } else if (query.length === 0) {
                        this.results.innerHTML = this.defaultCommands;
                    }
                });
            }

            // Handle command clicks
            if (this.results) {
                this.results.addEventListener('click', (e) => {
                    const item = e.target.closest('.command-item');
                    if (item) {
                        if (item.dataset.url) {
                            window.location.href = item.dataset.url;
                        } else if (item.dataset.action) {
                            this.executeCommand(item.dataset.action);
                        }
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
            if (this.results && this.defaultCommands) {
                this.results.innerHTML = this.defaultCommands;
            }
        },
        toggle() {
            if (this.el.classList.contains('active')) this.close();
            else this.open();
        },
        filterStatic(query) {
            if (!query) return;
            const q = query.toLowerCase();
            const items = this.results?.querySelectorAll('.command-item') || [];
            items.forEach(item => {
                const text = item.textContent.toLowerCase();
                item.style.display = text.includes(q) ? '' : 'none';
            });
        },
        async searchApi(query) {
            try {
                const res = await fetch(`/api/search?q=${encodeURIComponent(query)}`);
                if (!res.ok) return;
                const data = await res.json();
                const results = data.results || [];

                let html = `<div class="command-section-title">Actions</div>` + this.defaultCommands;
                
                if (results.length > 0) {
                    html = `<div class="command-section-title">Search Results (${results.length})</div>`;
                    results.forEach(r => {
                        html += `
                            <div class="command-item" data-url="${r.url}">
                                <div class="command-item-left">
                                    <span class="command-item-icon">${r.icon || '🔍'}</span>
                                    <div>
                                        <div class="command-item-title">${r.title}</div>
                                        <div class="command-item-sub">${r.sub || ''}</div>
                                    </div>
                                </div>
                                <span class="badge badge-neutral">${r.type}</span>
                            </div>
                        `;
                    });
                    html += `<div class="command-section-title" style="margin-top: 8px;">Quick Commands</div>` + this.defaultCommands;
                }

                if (this.results) {
                    this.results.innerHTML = html;
                    this.filterStatic(query);
                }
            } catch (err) {
                console.error('Search API error:', err);
            }
        },
        executeCommand(action) {
            const routes = {
                newTask: () => QuickCapture.show(),
                quickVoice: () => VoiceCapture.start(),
                newNote: '/Notes?action=create',
                newProject: '/Projects?action=create',
                newGoal: '/Goals?action=create',
                newHabit: '/Habits?action=create',
                startFocus: '/Focus',
                openToday: '/Today',
                openCalendar: '/Calendar',
                openUniversity: '/University',
                openDEPI: '/DEPI',
                openCareer: '/Career',
                openStudy: '/Study',
                openSkills: '/Skills',
                openSettings: '/Settings',
                planDay: '/Planner',
                reviewDay: '/Reviews',
                toggleTheme: () => ThemeManager.toggle()
            };
            const target = routes[action];
            if (typeof target === 'function') {
                target();
            } else if (typeof target === 'string') {
                window.location.href = target;
            }
        }
    };

    // ============================================================
    // QUICK CAPTURE & MODAL
    // ============================================================
    const QuickCapture = {
        init() {
            const btn = document.getElementById('quickCaptureBtn');
            const fab = document.getElementById('fabBtn');

            if (btn) btn.addEventListener('click', () => this.show());
            if (fab) fab.addEventListener('click', () => this.show());

            // Keyboard shortcut Ctrl+Shift+A
            document.addEventListener('keydown', (e) => {
                if (e.ctrlKey && e.shiftKey && e.key === 'A') {
                    e.preventDefault();
                    this.show();
                }
            });
        },
        show(initialText = '') {
            const modal = document.getElementById('globalQuickCaptureModal');
            const input = document.getElementById('globalQuickCaptureInput');
            if (modal) {
                modal.style.display = 'flex';
                if (input) {
                    input.value = initialText;
                    setTimeout(() => input.focus(), 80);
                }
            } else {
                window.location.href = `/Inbox?action=capture&title=${encodeURIComponent(initialText)}`;
            }
        },
        close() {
            const modal = document.getElementById('globalQuickCaptureModal');
            if (modal) modal.style.display = 'none';
        },
        async submit() {
            const input = document.getElementById('globalQuickCaptureInput');
            const statusSelect = document.getElementById('globalQuickCaptureStatus');
            const prioritySelect = document.getElementById('globalQuickCapturePriority');
            const btn = document.getElementById('saveQuickCaptureBtn');

            const title = input ? input.value.trim() : '';
            if (!title) return;

            if (btn) {
                btn.disabled = true;
                btn.textContent = 'Saving...';
            }

            try {
                const res = await fetch('/api/tasks/quick-create', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        title: title,
                        status: statusSelect ? statusSelect.value : 'Inbox',
                        priority: prioritySelect ? prioritySelect.value : 'Medium'
                    })
                });

                if (res.ok) {
                    if (window.AhmedOS && window.AhmedOS.Toast) {
                        window.AhmedOS.Toast.show(`Saved: "${title}"`, 'success', 3000);
                    }
                    this.close();
                    if (input) input.value = '';

                    // Reload page if currently on Tasks, Inbox, or Today
                    const path = window.location.pathname.toLowerCase();
                    if (path.includes('/tasks') || path.includes('/today') || path.includes('/inbox') || path === '/') {
                        setTimeout(() => location.reload(), 500);
                    }
                } else {
                    alert('Could not save task. Please try again.');
                }
            } catch (err) {
                console.error('QuickCapture submit error:', err);
            } finally {
                if (btn) {
                    btn.disabled = false;
                    btn.textContent = 'Save Task';
                }
            }
        }
    };

    // ============================================================
    // VOICE QUICK CAPTURE (Speech-to-Text)
    // ============================================================
    const VoiceCapture = {
        recognition: null,
        isListening: false,

        init() {
            const SpeechRec = window.SpeechRecognition || window.webkitSpeechRecognition;
            const topVoiceBtn = document.getElementById('voiceCaptureBtn');
            const modalVoiceBtn = document.getElementById('modalVoiceMicBtn');

            if (!SpeechRec) {
                if (topVoiceBtn) {
                    topVoiceBtn.title = 'Voice input not supported in this browser';
                    topVoiceBtn.style.opacity = '0.5';
                }
                return;
            }

            this.recognition = new SpeechRec();
            this.recognition.continuous = false;
            this.recognition.interimResults = true;
            this.recognition.lang = 'ar-EG'; // Primary Egyptian Arabic with English recognition

            this.recognition.onstart = () => {
                this.isListening = true;
                this.updateUi(true);
            };

            this.recognition.onresult = (event) => {
                let transcript = '';
                for (let i = event.resultIndex; i < event.results.length; ++i) {
                    transcript += event.results[i][0].transcript;
                }
                transcript = transcript.trim();
                if (transcript) {
                    QuickCapture.show(transcript);
                    const input = document.getElementById('globalQuickCaptureInput');
                    if (input) input.value = transcript;
                }
            };

            this.recognition.onerror = (event) => {
                console.warn('Speech recognition error:', event.error);
                this.isListening = false;
                this.updateUi(false);
                if (window.AhmedOS && window.AhmedOS.Toast) {
                    window.AhmedOS.Toast.show(`Voice: ${event.error}`, 'error', 3000);
                }
            };

            this.recognition.onend = () => {
                this.isListening = false;
                this.updateUi(false);
            };

            if (topVoiceBtn) topVoiceBtn.addEventListener('click', () => this.toggle());
            if (modalVoiceBtn) modalVoiceBtn.addEventListener('click', () => this.toggle());
        },

        start() {
            if (!this.recognition) {
                if (window.AhmedOS && window.AhmedOS.Toast) {
                    window.AhmedOS.Toast.show('Speech recognition is not supported in this browser.', 'warning', 4000);
                }
                return;
            }
            try {
                QuickCapture.show('');
                this.recognition.start();
            } catch (err) {
                console.warn('Recognition start issue:', err);
            }
        },

        stop() {
            if (this.recognition && this.isListening) {
                this.recognition.stop();
            }
        },

        toggle() {
            if (this.isListening) this.stop();
            else this.start();
        },

        updateUi(active) {
            const topVoiceBtn = document.getElementById('voiceCaptureBtn');
            const modalVoiceBtn = document.getElementById('modalVoiceMicBtn');
            const notice = document.getElementById('voiceStatusNotice');

            if (topVoiceBtn) topVoiceBtn.style.color = active ? 'var(--accent)' : '';
            if (modalVoiceBtn) modalVoiceBtn.style.color = active ? 'var(--accent)' : '';
            if (notice) notice.style.display = active ? 'block' : 'none';
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
    // NOTIFICATIONS CENTER
    // ============================================================
    const Notifications = {
        init() {
            const btn = document.getElementById('notificationsBtn');
            const dropdown = document.getElementById('notificationDropdown');
            const markAllBtn = document.getElementById('markAllReadBtn');
            const list = document.getElementById('notificationList');

            if (!btn || !dropdown) return;

            // Toggle dropdown
            btn.addEventListener('click', (e) => {
                e.stopPropagation();
                dropdown.classList.toggle('active');
                if (dropdown.classList.contains('active')) {
                    this.load();
                }
            });

            // Close on click outside
            document.addEventListener('click', (e) => {
                if (!dropdown.contains(e.target) && e.target !== btn) {
                    dropdown.classList.remove('active');
                }
            });

            // Mark all as read
            if (markAllBtn) {
                markAllBtn.addEventListener('click', async (e) => {
                    e.stopPropagation();
                    try {
                        const res = await fetch('/api/notifications/read-all', { method: 'POST' });
                        if (res.ok) {
                            Toast.show('All notifications marked as read', 'success');
                            this.load();
                            const dot = btn.querySelector('.badge-dot');
                            if (dot) dot.style.display = 'none';
                        }
                    } catch (err) {
                        console.error('Error marking notifications as read:', err);
                    }
                });
            }

            // Initial check for unread count
            this.checkUnread();
        },
        async checkUnread() {
            try {
                const res = await fetch('/api/notifications/unread-count');
                if (!res.ok) return;
                const data = await res.json();
                const dot = document.getElementById('notificationsBtn')?.querySelector('.badge-dot');
                if (dot) {
                    dot.style.display = data.count > 0 ? 'block' : 'none';
                }
            } catch (err) {
                // Ignore silent background check error
            }
        },
        async load() {
            const list = document.getElementById('notificationList');
            if (!list) return;

            try {
                const res = await fetch('/api/notifications');
                if (!res.ok) return;
                const items = await res.json();

                if (items.length === 0) {
                    list.innerHTML = `<div style="padding: 24px; text-align: center; color: var(--text-tertiary); font-size: 13px;">No notifications yet ✨</div>`;
                    return;
                }

                let html = '';
                items.forEach(n => {
                    const unreadClass = !n.isRead ? 'unread' : '';
                    const icon = n.type === 'Warning' ? '⚠️' : n.type === 'Alert' ? '🚨' : n.type === 'Reminder' ? '⏰' : '🔔';
                    html += `
                        <a href="${n.actionUrl || '#'}" class="notification-item ${unreadClass}" data-id="${n.id}">
                            <span class="notification-item-icon">${icon}</span>
                            <div class="notification-item-content">
                                <div class="notification-item-title">${n.title}</div>
                                <div class="notification-item-msg">${n.message || ''}</div>
                                <div class="notification-item-time">${n.timeAgo}</div>
                            </div>
                        </a>
                    `;
                });
                list.innerHTML = html;

                // Handle item clicks to mark as read
                list.querySelectorAll('.notification-item').forEach(el => {
                    el.addEventListener('click', async () => {
                        const id = el.dataset.id;
                        if (id) {
                            try {
                                await fetch(`/api/notifications/${id}/read`, { method: 'POST' });
                            } catch (e) {}
                        }
                    });
                });
            } catch (err) {
                list.innerHTML = `<div style="padding: 16px; text-align: center; color: var(--color-danger); font-size: 12px;">Failed to load notifications</div>`;
            }
        }
    };

    // ============================================================
    // AI ASSISTANT DRAWER
    // ============================================================
    const AiAssistant = {
        init() {
            const btn = document.getElementById('aiDrawerBtn');
            const drawer = document.getElementById('aiDrawer');
            const closeBtn = document.getElementById('closeAiDrawerBtn');
            const sendBtn = document.getElementById('sendAiBtn');
            const input = document.getElementById('aiInput');
            const messages = document.getElementById('aiMessages');

            if (!drawer) return;

            const toggle = () => {
                drawer.classList.toggle('open');
                if (drawer.classList.contains('open')) {
                    setTimeout(() => input?.focus(), 150);
                }
            };

            if (btn) btn.addEventListener('click', toggle);
            if (closeBtn) closeBtn.addEventListener('click', () => drawer.classList.remove('open'));

            // Shortcut: Ctrl+J / Cmd+J
            document.addEventListener('keydown', (e) => {
                if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'j') {
                    e.preventDefault();
                    toggle();
                }
            });

            const send = async () => {
                const text = input?.value?.trim();
                if (!text || !messages) return;

                // Append user message
                const userMsg = document.createElement('div');
                userMsg.className = 'ai-msg ai-msg-user';
                userMsg.textContent = text;
                messages.appendChild(userMsg);
                input.value = '';
                messages.scrollTop = messages.scrollHeight;

                // Loading indicator
                const botMsg = document.createElement('div');
                botMsg.className = 'ai-msg ai-msg-bot';
                botMsg.textContent = 'Thinking... ✨';
                messages.appendChild(botMsg);
                messages.scrollTop = messages.scrollHeight;

                try {
                    const res = await fetch('/api/ai/chat', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ message: text })
                    });
                    if (res.ok) {
                        const data = await res.json();
                        botMsg.innerHTML = data.reply.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>').replace(/\n/g, '<br/>');
                    } else {
                        botMsg.textContent = "Sorry, I couldn't process that request right now.";
                    }
                } catch (err) {
                    botMsg.textContent = "Connection error. Please try again.";
                }
                messages.scrollTop = messages.scrollHeight;
            };

            if (sendBtn) sendBtn.addEventListener('click', send);
            if (input) {
                input.addEventListener('keydown', (e) => {
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        send();
                    }
                });
            }
        }
    };

    // ============================================================
    // MODAL MANAGER
    // ============================================================
    const ModalManager = {
        init() {
            // Close when clicking modal backdrop
            document.addEventListener('click', (e) => {
                if (e.target.classList.contains('modal-backdrop')) {
                    e.target.classList.remove('active');
                }
            });

            // Close when pressing Escape
            document.addEventListener('keydown', (e) => {
                if (e.key === 'Escape') {
                    document.querySelectorAll('.modal-backdrop.active').forEach(m => m.classList.remove('active'));
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
            if (todayBadge) todayBadge.style.display = 'none';
            if (inboxBadge) inboxBadge.style.display = 'none';
        }
    };

    // Expose APIs on window.AhmedOS
    window.AhmedOS = window.AhmedOS || {};
    window.AhmedOS.Toast = Toast;
    window.AhmedOS.QuickCapture = QuickCapture;
    window.AhmedOS.VoiceCapture = VoiceCapture;
    window.AhmedOS.CommandPalette = CommandPalette;
    window.AhmedOS.Theme = ThemeManager;

    // ============================================================
    // INITIALIZE
    // ============================================================
    document.addEventListener('DOMContentLoaded', () => {
        ThemeManager.init();
        Sidebar.init();
        CommandPalette.init();
        QuickCapture.init();
        VoiceCapture.init();
        TaskActions.init();
        Greeting.init();
        Clock.init();
        HabitToggle.init();
        Tabs.init();
        Notifications.init();
        AiAssistant.init();
        ModalManager.init();
        Badges.init();
    });
})();
