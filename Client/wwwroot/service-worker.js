// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
self.addEventListener('install', event => {
    event.waitUntil(
        caches.open('blazor-cache-v1').then(cache => {
            return cache.addAll([
                '/',
                'index.html',
                'manifest.json',
                '_framework/blazor.webassembly.js',
                '_framework/dotnet.wasm',
                // Add other assets you need to cache
            ]);
        })
    );
});

self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request).then(response => {
            return response || fetch(event.request);
        })
    );
});

