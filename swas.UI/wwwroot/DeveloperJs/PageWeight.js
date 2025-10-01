
/* ===== Perf Overlay + Cache Reset (jQuery) ===== */
    (function ($) {
        $.ajaxSetup({ cache: false });

    // ---------- UI ----------
    var $box = $('<div />', {
        css: {
        position: 'fixed', right: '12px', bottom: '12px', zIndex: 999999,
    background: '#111', color: '#eee', padding: '10px 12px',
    borderRadius: '10px', font: '12px/1.4 system-ui,Segoe UI,Arial',
    boxShadow: '0 6px 20px rgba(0,0,0,.35)', maxWidth: '360px'
    }
  }).text('⏱ Measuring…');

    var $btn = $('<button />', {
        text: 'Reset Cache & Reload',
    css: {
        marginTop: '8px', width: '100%', padding: '6px 8px',
    background: '#2a7', color: '#fff', border: 0, borderRadius: '8px',
    cursor: 'pointer'
    },
    click: resetCachesAndHardReload
  });

    $(function () {$('body').append($box.append('<br/>', $btn)); });

    // ---------- Helpers ----------
    function formatBytes(bytes) {
    if (!bytes || bytes < 0) return '0 B';
    var units = ['B', 'KB', 'MB', 'GB'];
    var i = Math.min(Math.floor(Math.log(bytes) / Math.log(1024)), units.length - 1);
    return (bytes / Math.pow(1024, i)).toFixed(i ? 2 : 0) + ' ' + units[i];
  }
    function ms(x) { return Math.max(0, Math.round(x)) + ' ms'; }

    function sumResourceBytes(entries) {
    var total = 0, byType = { };
    entries.forEach(function (e) {
      var size = (e.transferSize && e.transferSize > 0)
    ? e.transferSize
        : ((e.encodedBodySize && e.encodedBodySize > 0) ? e.encodedBodySize : 0);
    total += size;
    var t = e.initiatorType || 'other';
    byType[t] = (byType[t] || 0) + size;
    });
    return {total: total, byType: byType };
  }

    var lastLcpEl = null;
    function highlightLcpElement(el) {
    if (!el || !(el instanceof Element)) return;
    lastLcpEl = el;
    el.style.outline = '2px dashed #2a7';
    el.style.outlineOffset = '2px';
  }

    // ---------- Main collector (single definition!) ----------
    function collectNow() {
    var nav = performance.getEntriesByType('navigation')[0];
    var res = performance.getEntriesByType('resource');

    var sums = sumResourceBytes(res);
    var totalRes = sums.total, byType = sums.byType;

    // HTML bytes (transferSize includes headers when available)
    var htmlBytes = 0, htmlBody = 0;
    if (nav) {
        htmlBytes = nav.transferSize > 0 ? nav.transferSize :
            (nav.encodedBodySize > 0 ? nav.encodedBodySize : 0);
    htmlBody = nav.encodedBodySize || 0;
    }

    // Estimate headers/TLS overhead across all requests (transfer - encodedBody)
    var overheadBytes = 0;
    res.forEach(function (e) {
      var t = (e.transferSize || 0), b = (e.encodedBodySize || 0);
      if (t > 0 && b > 0 && t > b) overheadBytes += (t - b);
    });
    if (nav && nav.transferSize > 0 && htmlBody > 0 && nav.transferSize > htmlBody) {
        overheadBytes += (nav.transferSize - htmlBody);
    }

    var ttfb = nav ? nav.responseStart - nav.requestStart : 0;
    var dcl  = nav ? nav.domContentLoadedEventEnd - nav.startTime : 0;
    var load = nav ? nav.loadEventEnd - nav.startTime : 0;
    var lcp  = window.__LCP__ || 0;

    var grand = htmlBytes + totalRes; // totals include headers via transferSize when available

    // Show all initiator types present
    var allTypes = Object.keys(byType).sort(function(a,b){ return byType[b]-byType[a]; });
    var lines = allTypes.map(function (t) { return t + ': ' + formatBytes(byType[t]); });

    var netInfo = (navigator.connection && (navigator.connection.effectiveType || navigator.connection.downlink))
    ? ('<br />Net: ' + (navigator.connection.effectiveType || '') +
    (navigator.connection.downlink ? ' (' + navigator.connection.downlink + ' Mbps)' : ''))
    : '';

    $box.html(
    '📄 HTML: <b>' + formatBytes(htmlBytes) + '</b><br />' +
    '📦 Page weight: <b>' + formatBytes(grand) + '</b><br />' +
    (lines.length ? '• ' + lines.join(' · ') + '<br />' : '') +
    '🧾 Headers/TLS overhead (est): <b>' + formatBytes(overheadBytes) + '</b><br />' +
    '⏱ TTFB: <b>' + ms(ttfb) + '</b> · DCL: <b>' + ms(dcl) + '</b> · Load: <b>' + ms(load) + '</b>' +
    (lcp ? ' · LCP: <b>' + ms(lcp) + '</b>' : '') +
    netInfo
    ).append('<br />').append($btn);

    scheduleConsolePrint(htmlBytes, byType, grand, nav);
  }

    // Observe new resources & LCP
    if ('PerformanceObserver' in window) {
    try {
      var po = new PerformanceObserver(function(){collectNow(); });
    po.observe({type: 'resource', buffered: true });
    } catch(e){ }
    try {
      var lcpObs = new PerformanceObserver(function(list){
        var entries = list.getEntries();
    var last = entries[entries.length - 1];
    if (last) {
        window.__LCP__ = (last.renderTime || last.loadTime || 0);
    if (last.element) highlightLcpElement(last.element);
        }
      });
    lcpObs.observe({type: 'largest-contentful-paint', buffered: true });
    } catch(e){ }
  }

    // Final update after full load + a few refreshes for lazy assets
    $(window).on('load', function(){
        setTimeout(collectNow, 0);
    var n=0, iv=setInterval(function(){collectNow(); if(++n>=5) clearInterval(iv); }, 1000);
  });

    // Expose manual refresh (for SPA or dev)
    window.__perfOverlayRefresh = collectNow;

    // ---------- Cache reset & hard reload ----------
    async function resetCachesAndHardReload() {
        $btn.prop('disabled', true).text('Clearing caches…');
    try {
      if (window.caches && caches.keys) {
        var keys = await caches.keys();
    await Promise.all(keys.map(function(k){ return caches.delete(k); }));
      }
    if (navigator.serviceWorker && navigator.serviceWorker.getRegistrations) {
        var regs = await navigator.serviceWorker.getRegistrations();
    await Promise.all(regs.map(function(r){ return r.unregister(); }));
      }
    try {localStorage.clear(); } catch(e){ }
    try {sessionStorage.clear(); } catch(e){ }

    var url = new URL(location.href);
    url.searchParams.set('_nocache', Date.now().toString());
    location.replace(url.toString());
    } catch (err) {
        console.error('Cache reset error:', err);
    alert('Tried to clear caches, but the browser blocked some steps. Reloading with cache-buster.');
    var u = new URL(location.href); u.searchParams.set('_nocache', Date.now().toString());
    location.replace(u.toString());
    }
  }

    // ---------- Add cache-buster on internal links ----------
    $(document).on('click', 'a[href]', function (e) {
    var href = $(this).attr('href');
    if (!href || href.startsWith('#') || href.startsWith('javascript:')) return;
    var url;
    try {url = new URL(href, location.href); } catch (_) { return; }
    if (url.origin !== location.origin) return;
    url.searchParams.set('_nocache', Date.now().toString());
    e.preventDefault();
    location.href = url.toString();
  });

    // ---------- Console output (summary + Top 10) ----------
    var consoleDebounce = null;
    function scheduleConsolePrint(htmlBytes, byType, grand, nav) {
        clearTimeout(consoleDebounce);
    consoleDebounce = setTimeout(function(){
      try {
        var summary = {URL: location.href, HTML: formatBytes(htmlBytes) };
    Object.keys(byType).sort(function(a,b){ return byType[b]-byType[a]; })
    .forEach(function(k){summary[k.toUpperCase()] = formatBytes(byType[k]); });
    summary.TOTAL = formatBytes(grand);

    console.groupCollapsed('Page Perf Summary');
    console.table(summary);
    console.log(
    'TTFB:', (nav && nav.responseStart && nav.requestStart) ? ms(nav.responseStart - nav.requestStart) : 'n/a',
    'DCL:',  (nav && nav.domContentLoadedEventEnd) ? ms(nav.domContentLoadedEventEnd - nav.startTime) : 'n/a',
    'Load:', (nav && nav.loadEventEnd) ? ms(nav.loadEventEnd - nav.startTime) : 'n/a',
    'LCP:',  window.__LCP__ ? ms(window.__LCP__) : 'n/a'
    );
    if (lastLcpEl) console.log('LCP element:', lastLcpEl);

    var list = performance.getEntriesByType('resource').map(function(e){
          var size = (e.transferSize>0?e.transferSize:(e.encodedBodySize||0));
    return {TYPE: (e.initiatorType||'other'), SIZE: size, URL: e.name };
        }).sort(function(a,b){ return b.SIZE - a.SIZE; }).slice(0,10)
    .map(function(r){ return {TYPE: r.TYPE, SIZE: formatBytes(r.SIZE), URL: r.URL }; });
    console.table(list);
    console.log('Note: Exact transferSize for cross-origin requires Timing-Allow-Origin from the CDN.');
    console.groupEnd();
      } catch (e) { }
    }, 250);
  }

})(jQuery);

