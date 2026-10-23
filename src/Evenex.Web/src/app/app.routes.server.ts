import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  // Giriş gerektiren / kullanıcıya özel sayfalar sadece tarayıcıda çizilir (RenderMode.Client).
  // Build sırasında önceden çizilseydi (Prerender) sunucuda token olmadığı için guard hep
  // "giriş yapılmamış" sanıp login'e yönlendiren bir sayfa üretirdi.
  { path: 'admin/**', renderMode: RenderMode.Client },
  { path: 'auth/**', renderMode: RenderMode.Client },
  {
    path: '**',
    renderMode: RenderMode.Prerender
  }
];
