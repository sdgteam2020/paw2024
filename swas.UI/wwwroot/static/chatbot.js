
    (function () {
  const dialog = document.getElementById('asdcChat');
    const handle = dialog.querySelector('.asdc-chat-header');

    let dragging = false;
    let startX = 0, startY = 0;      // mouse/touch position at mousedown
    let startLeft = 0, startTop = 0; // dialog position at mousedown

    // compute current numeric left/top of dialog
    function getDialogRect() {
    const r = dialog.getBoundingClientRect();
    // when bottom/right were used initially, convert to left/top now
    if (dialog.style.left === '' && dialog.style.right !== '') {
        dialog.style.left = (window.innerWidth - r.right) + 'px';
    dialog.style.top  = (r.top) + 'px';
    dialog.style.right = 'auto';
    dialog.style.bottom = 'auto';
    }
    return dialog.getBoundingClientRect();
  }

    function onStart(clientX, clientY) {
    if (dialog.classList.contains('fullscreen')) return; // no drag in fullscreen
    dragging = true;
    const rect = getDialogRect();
    startX = clientX;
    startY = clientY;
    startLeft = rect.left;
    startTop  = rect.top;

    // prevent text selection while dragging
    document.body.style.userSelect = 'none';
  }

    function onMove(clientX, clientY) {
    if (!dragging) return;
    const dx = clientX - startX;
    const dy = clientY - startY;

    // new pos
    let newLeft = startLeft + dx;
    let newTop  = startTop  + dy;

    // keep inside viewport
    const rect = dialog.getBoundingClientRect();
    const maxLeft = window.innerWidth  - rect.width;
    const maxTop  = window.innerHeight - rect.height;

    newLeft = Math.max(0, Math.min(newLeft, maxLeft));
    newTop  = Math.max(0, Math.min(newTop,  maxTop));

    dialog.style.left = newLeft + 'px';
    dialog.style.top  = newTop  + 'px';
    dialog.style.right = 'auto';
    dialog.style.bottom = 'auto';
  }

    function onEnd() {
        dragging = false;
    document.body.style.userSelect = '';
  }

  // Mouse events
  handle.addEventListener('mousedown', (e) => onStart(e.clientX, e.clientY));
  document.addEventListener('mousemove', (e) => onMove(e.clientX, e.clientY));
    document.addEventListener('mouseup', onEnd);

  // Touch events (mobile)
  handle.addEventListener('touchstart', (e) => {
    const t = e.touches[0];
    onStart(t.clientX, t.clientY);
  }, {passive: true });

  document.addEventListener('touchmove', (e) => {
    if (!dragging) return;
    const t = e.touches[0];
    onMove(t.clientX, t.clientY);
  }, {passive: true });

    document.addEventListener('touchend', onEnd);

    // If you toggle fullscreen, reset position when exiting
    const fullscreenBtn = document.getElementById('asdcFullscreen');
  fullscreenBtn?.addEventListener('click', () => {
    if (!dialog.classList.contains('fullscreen')) {
        // leaving fullscreen → snap to bottom-right baseline again
        dialog.style.bottom = '20px';
    dialog.style.right  = '20px';
    dialog.style.left   = '';
    dialog.style.top    = '';
    }
  });
})();

    document.addEventListener("DOMContentLoaded", function() {
  const button = document.getElementById("asdcChatToggle");
    const tooltip = button.querySelector(".tooltip-bubble");

    button.addEventListener("mouseenter", function() {
    const rect = button.getBoundingClientRect();
    const spaceAbove = rect.top;
    const spaceBelow = window.innerHeight - rect.bottom;

    if (spaceAbove < 60 && spaceBelow > spaceAbove) {
        tooltip.setAttribute("data-position", "bottom");
    } else {
        tooltip.setAttribute("data-position", "top");
    }
  });
});

    const chat = document.getElementById("asdcChat");
    const fullscreenBtn = document.getElementById("asdcFullscreen");

fullscreenBtn.addEventListener("click", () => {
        chat.classList.toggle("fullscreen");
    fullscreenBtn.textContent = chat.classList.contains("fullscreen") ? "🗗" : "⛶"; 
});

    const btn = document.getElementById("asdcChatToggle");
    let isDragging = false, offsetX, offsetY;

  btn.addEventListener("mousedown", (e) => {
        isDragging = true;
    offsetX = e.clientX - btn.getBoundingClientRect().left;
    offsetY = e.clientY - btn.getBoundingClientRect().top;
    btn.style.cursor = "grabbing";
  });

  document.addEventListener("mousemove", (e) => {
    if (!isDragging) return;
    btn.style.left = (e.clientX - offsetX) + "px";
    btn.style.top = (e.clientY - offsetY) + "px";
    btn.style.right = "auto";  // prevent snapping back
    btn.style.bottom = "auto";
  });

  document.addEventListener("mouseup", () => {
        isDragging = false;
    btn.style.cursor = "grab";
  });
    // --- State ---
    let isOpen = false;
    let isSending = false;
    let welcomeShown = false;
    let lastUserQuery = "";
    let heartbeatTimer = null;

    // Elements
    const dlg        = document.getElementById('asdcChat');
    const toggleBtn  = document.getElementById('asdcChatToggle');
    const closeBtn   = document.getElementById('asdcClose');
    const chatBox    = document.getElementById('asdcChatBox');
    const inputEl    = document.getElementById('asdcQuery');
    const sendBtn    = document.getElementById('asdcSend');
    const loadingEl  = document.getElementById('asdcLoading');

    // Focus-trap support
    function focusableElements(root) {
    return Array.from(root.querySelectorAll(
    'button, [href], input, textarea, select, [tabindex]:not([tabindex="-1"])'
    )).filter(el => !el.disabled && el.offsetParent !== null);
  }

    function openChat() {
    if (isOpen) return;
    dlg.hidden = false;
    toggleBtn.setAttribute('aria-expanded', 'true');
    isOpen = true;

    // First-time welcome
    if (!welcomeShown) {
        showWelcome();
    welcomeShown = true;
    }

    // Start heartbeat while open (every 4 min)

    // Focus management
    const f = focusableElements(dlg);
    if (f.length) f[0].focus();

    // Scroll to last
    requestAnimationFrame(() => {
        chatBox.scrollTop = chatBox.scrollHeight;
    });

    // Close on ESC
    document.addEventListener('keydown', onEsc, {capture: true });
    dlg.addEventListener('keydown', trapFocus);
  }

    function closeChat() {
    if (!isOpen) return;
    dlg.hidden = true;
    toggleBtn.setAttribute('aria-expanded', 'false');
    isOpen = false;

    // Stop heartbeat
    if (heartbeatTimer) {
        clearInterval(heartbeatTimer);
    heartbeatTimer = null;
    }

    // Return focus to toggle
    toggleBtn.focus();

    document.removeEventListener('keydown', onEsc, {capture: true });
    dlg.removeEventListener('keydown', trapFocus);
  }

    function onEsc(e) {
    if (e.key === 'Escape') {
        closeChat();
    e.preventDefault();
    }
  }

    function trapFocus(e) {
    if (e.key !== 'Tab') return;
    const f = focusableElements(dlg);
    if (!f.length) return;
    const first = f[0];
    const last = f[f.length - 1];
    if (e.shiftKey && document.activeElement === first) {
        last.focus();
    e.preventDefault();
    } else if (!e.shiftKey && document.activeElement === last) {
        first.focus();
    e.preventDefault();
    }
  }

  // Toggle
// Toggle
toggleBtn.addEventListener('click', () => {
  if (isOpen) {
        closeChat();
  } else {
        openChat();
    sendQuery('warmup', true); // silent ping

  }

    // ✅ Start heartbeat on first toggle click
    if (!heartbeatTimer) {
        heartbeatTimer = setInterval(() => {
            sendQuery('warmup', true); // silent ping
        }, 240000);
  }
});
    closeBtn.addEventListener('click', closeChat);

  // Submit handlers
  sendBtn.addEventListener('click', () => sendQuery());
  inputEl.addEventListener('keydown', (e) => {
    if (e.key === 'Enter') sendQuery();
  });

    // Dynamic viewport height var (mobile keyboards)
    function setVhVar() {
    const vh = window.visualViewport ? window.visualViewport.height : window.innerHeight;
    document.documentElement.style.setProperty('--asdc-vh', vh + 'px');
  }
    setVhVar();
    window.addEventListener('resize', setVhVar);
    if (window.visualViewport) window.visualViewport.addEventListener('resize', setVhVar);

    // --- UI helpers (vanilla, no Tailwind) ---
    function scrollToBottom(el, smooth = true) {
    const nearBottom = el.scrollHeight - el.scrollTop - el.clientHeight < 80;
    el.scrollTo({top: el.scrollHeight, behavior: smooth && nearBottom ? 'smooth' : 'auto' });
  }

    let stopStreaming = false;

    function streamText(container, text, speed = 1, onDone, onChunk) {
        let i = 0;
    (function tick() {
    if (stopStreaming) {
        stopStreaming = false; // reset for next use
    return; // stop streaming immediately
    }
    if (i < text.length) {
      const ch = text[i] === "\n" ? "<br>" : text[i];
        container.innerHTML += ch;
        if (onChunk) onChunk();
        i++;
        setTimeout(tick, speed);
    } else {
      if (onDone) onDone();
    }
  })();
}

        // --- Unified feedback block: meta → controls → 👍 → 👎 ---
        function appendFeedbackBlock(container, meta, role, text, textDiv) {
  const feedbackDiv = document.createElement('div');
        feedbackDiv.className = 'asdc-feedback';

        // ---- Controls container
        const controls = document.createElement('div');
        controls.className = 'asdc-controls';

        // ---- Common: Copy button
        const copyBtn = document.createElement('button');
        copyBtn.type = 'button';
        copyBtn.className = 'asdc-icon-btn';
        copyBtn.title = 'Copy';
        copyBtn.textContent = '📋';
  copyBtn.onclick = () => {
            navigator.clipboard.writeText(text);
        copyBtn.textContent = '✅';
    setTimeout(() => (copyBtn.textContent = '📋'), 1500);
  };

        if (role === 'user') {
    // ✅ USER: Only Edit + Copy
    const editBtn = document.createElement('button');
        editBtn.type = 'button';
        editBtn.className = 'asdc-icon-btn';
        editBtn.title = 'Edit & resend';
        editBtn.textContent = '✏️';
    editBtn.onclick = () => makeEditable(textDiv, text);

        controls.appendChild(editBtn);
        controls.appendChild(copyBtn);

  } else {
    // 🤖 BOT: Like/Dislike (+ you can keep Copy or remove if you want)
    const like = document.createElement('button');
        like.type = 'button';
        like.className = 'asdc-icon-btn';
        like.title = 'Like';
        like.textContent = '👍';

        const dislike = document.createElement('button');
        dislike.type = 'button';
        dislike.className = 'asdc-icon-btn';
        dislike.title = 'Dislike';
        dislike.textContent = '👎';

        // If you do NOT want Copy for bot, remove next line:
        controls.appendChild(copyBtn);

    feedbackDiv.addEventListener('click', (ev) => {
      if (ev.target === like) rateResponse('like');
        if (ev.target === dislike) rateResponse('dislike');
    });

        feedbackDiv.appendChild(like);
        feedbackDiv.appendChild(dislike);
  }

        // ---- Meta (optional, usually for bot)
        if (meta && (meta.duration || meta.scores)) {
    const metaDiv = document.createElement('div');
        metaDiv.className = 'asdc-meta';

        if (meta.duration) {
      const duration = Number(meta.duration).toFixed(2);
        const timeSpan = document.createElement('span');
        timeSpan.textContent = `⏱️ ${duration}s`;
        timeSpan.title = `Response time: ${duration} seconds`;
        metaDiv.appendChild(timeSpan);
    }
        if (meta.scores) {
      const scoreSpan = document.createElement('span');
        if (typeof meta.scores === 'object') {
        const vals = Object.values(meta.scores).map(v => Number(v).toFixed(2));
        scoreSpan.textContent = ` 🔍 ${vals.join(", ")}`;
        scoreSpan.title = `Similarity scores: ${vals.join(", ")}`;
      } else {
        const score = Number(meta.scores).toFixed(2);
        scoreSpan.textContent = ` 🔍 ${score}`;
        scoreSpan.title = `Similarity score: ${score}`;
      }
        metaDiv.appendChild(scoreSpan);
    }
        feedbackDiv.appendChild(metaDiv);
  }

        // Append controls last so they align to the right
        feedbackDiv.appendChild(controls);
        container.appendChild(feedbackDiv);
}

        // --- MAIN: addMessage using unified feedback block ---
        function addMessage(role, text, options = { }) {
    const {stream = false, link = "", meta = null} = options;

        const row = document.createElement('div');
        row.className = role === 'user' ? 'asdc-row asdc-row--me' : 'asdc-row asdc-row--bot';

        const icon = document.createElement('div');
        icon.className = 'asdc-icon';
        icon.setAttribute('aria-hidden', 'true');
        if (role === 'user') {
            icon.innerHTML = `<img src="/static/user4.png"  alt="User" height="20" width="26"/>`;
} else {
            // Inline SVG for robot
            icon.innerHTML = `
   <svg xmlns="http://www.w3.org/2000/svg" height="18" width="24" viewBox="0 0 640 512" fill="green">
  <path d="M320 0c17.7 0 32 14.3 32 32l0 64 120 0c39.8 0 72 32.2 72 72l0 272c0 39.8-32.2 72-72 72l-304 0c-39.8 0-72-32.2-72-72l0-272c0-39.8 32.2-72 72-72l120 0 0-64c0-17.7 14.3-32 32-32zM208 384c-8.8 0-16 7.2-16 16s7.2 16 16 16l32 0c8.8 0 16-7.2 16-16s-7.2-16-16-16l-32 0zm96 0c-8.8 0-16 7.2-16 16s7.2 16 16 16l32 0c8.8 0 16-7.2 16-16s-7.2-16-16-16l-32 0zm96 0c-8.8 0-16 7.2-16 16s7.2 16 16 16l32 0c8.8 0 16-7.2 16-16s-7.2-16-16-16l-32 0zM264 256a40 40 0 1 0 -80 0 40 40 0 1 0 80 0zm152 40a40 40 0 1 0 0-80 40 40 0 1 0 0 80zM48 224l16 0 0 192-16 0c-26.5 0-48-21.5-48-48l0-96c0-26.5 21.5-48 48-48zm544 0c26.5 0 48 21.5 48 48l0 96c0 26.5-21.5 48-48 48l-16 0 0-192 16 0z"/>
</svg>
    `;
}

        const bubble = document.createElement('div');
        bubble.className = role === 'user' ? 'asdc-bubble asdc-bubble--me' : 'asdc-bubble asdc-bubble--bot';

        const contentWrap = document.createElement('div');
        contentWrap.className = 'asdc-bubble-content';

        const textDiv = document.createElement('div');
        textDiv.className = 'asdc-text';
        textDiv.innerHTML = stream ? '' : text;

        contentWrap.appendChild(textDiv);

        if (link) {
      const linkDiv = document.createElement('div');
        linkDiv.className = 'asdc-link';
        linkDiv.innerHTML = link;
        contentWrap.appendChild(linkDiv);
    }

        // USER: (no feedback block unless you want it — uncomment to enable)
        if (role === 'user') {
            appendFeedbackBlock(contentWrap, null, role, text, textDiv);
    }

        if (role === 'bot') {
  if (stream) {
    // Create stop button
    const stopBtn = document.createElement('button');
        stopBtn.type = 'button';
        stopBtn.className = 'asdc-icon-btn asdc-stop-btn';
        stopBtn.title = 'Stop response';
        stopBtn.textContent = '⏹️';

    // when clicked, set stop flag and remove button
    stopBtn.onclick = () => {
            stopStreaming = true;
        stopBtn.remove();
    };

        contentWrap.appendChild(stopBtn);

        streamText(
        textDiv,
        text,
        1,
      () => {
            stopBtn.remove(); // remove button after finish
        appendFeedbackBlock(contentWrap, meta, role, text, textDiv);
        scrollToBottom(chatBox);
      },
      () => scrollToBottom(chatBox)
        );
  } else {
            appendFeedbackBlock(contentWrap, meta, role, text, textDiv);
  }
}

        bubble.appendChild(contentWrap);

        if (role === 'user') {
            row.appendChild(bubble);
        row.appendChild(icon);
    } else {
            row.appendChild(icon);
        row.appendChild(bubble);
    }

        chatBox.appendChild(row);
    requestAnimationFrame(() => scrollToBottom(chatBox));
  }

        // --- Edit UI ---
        function makeEditable(textDiv, originalText) {
    const wrapper = document.createElement('div');
        wrapper.className = 'asdc-edit-wrap';

        const input = document.createElement('input');
        input.type = 'text';
        input.value = originalText;
        input.className = 'asdc-edit-input';

        const btns = document.createElement('div');
        btns.className = 'asdc-edit-btns';

        const cancel = document.createElement('button');
        cancel.type = 'button';
        cancel.className = 'asdc-btn asdc-btn--gray';
        cancel.textContent = 'Cancel';
    cancel.onclick = () => {
            textDiv.textContent = originalText;
        wrapper.remove();
    };

        const send = document.createElement('button');
        send.type = 'button';
        send.className = 'asdc-btn asdc-btn--blue';
        send.textContent = 'Send';
    send.onclick = () => {
      const val = input.value.trim();
        if (val) {
            textDiv.textContent = val;
        wrapper.remove();
        sendQuery(val);
      }
    };

        btns.appendChild(cancel);
        btns.appendChild(send);
        wrapper.appendChild(input);
        wrapper.appendChild(btns);

        // replace current text with editor
        textDiv.textContent = '';
        textDiv.appendChild(wrapper);
        input.focus();
  }

        function showWelcome() {
            addMessage('bot', '👋 Welcome! How can I assist you today?');
  }

        function rateResponse(type) {
    const msg = document.createElement('div');
        msg.className = 'asdc-bubble asdc-bubble--note';
        msg.textContent = type === 'like' ? '👍 Thanks for liking the response!' : '👎 Thanks for your feedback!';
        chatBox.appendChild(msg);
        scrollToBottom(chatBox);

        fetch("https://dgis.army.mil/chatbot/asdcfeedback/", {
            method: "POST",
        headers: {"Content-Type": "application/json" },
        body: JSON.stringify({feedback: type, query: lastUserQuery, collection_name: "chatbot_paw" })
    }).catch(() => { });
  }

        // --- Network ---
        async function sendQuery(optionalQuery = null, silent = false) {
    if (isSending) return;

        const userQuery = (optionalQuery ?? inputEl.value).trim();
        if (!userQuery) return;

        isSending = true;
        lastUserQuery = userQuery;

        if (!silent) {
            addMessage('user', userQuery);
        inputEl.value = '';
        inputEl.disabled = true;
        sendBtn.disabled = true;
        loadingEl.hidden = false;
        chatBox.setAttribute('aria-busy', 'true');
    }

        const t0 = performance.now();
        try {
      const res = await fetch("https://dgis.army.mil/chatbot/asdcask/", {
            method: "POST",
        headers: {"Content-Type": "application/json" },
        body: JSON.stringify({query: userQuery, collection_name: "chatbot_paw" })
      });
        const data = await res.json();
        const t1 = performance.now();
        const roundTrip = (t1 - t0) / 1000;

        if (!silent) {
            loadingEl.hidden = true;
        chatBox.setAttribute('aria-busy', 'false');
        addMessage(
        'bot',
        data.answer || "Please ask a question related to the website.",
        {stream: true, link: data.link || "", meta: {duration: data.duration ?? roundTrip, scores: data.scores ?? null } }
        );
      }
    } catch {
      if (!silent) {
            loadingEl.hidden = true;
        chatBox.setAttribute('aria-busy', 'false');
        addMessage('bot', '❌ Error processing your request.');
      }
    } finally {
            isSending = false;
        if (!silent) {
            inputEl.disabled = false;
        sendBtn.disabled = false;
        inputEl.focus();
      }
    }
  }

