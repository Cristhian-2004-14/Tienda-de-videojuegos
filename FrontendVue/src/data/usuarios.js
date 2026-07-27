// Repository: capa de acceso a datos mock de Usuario.
// TODO: reemplazar por fetch a /api/usuario cuando el backend esté listo.
// La contraseña es solo para validar el login mock (sección 9 de CLAUDE.md: sin auth real).
export const usuarios = [
  {
    id: 1,
    username: 'admin',
    password: 'admin123',
    rol: 'Administrador',
  },
  {
    id: 2,
    username: 'jperez',
    password: 'vendedor123',
    rol: 'Vendedor',
  },
  {
    id: 3,
    username: 'mrodriguez',
    password: 'tecnico123',
    rol: 'Técnico',
  },
  {
    id: 4,
    username: 'lgomez',
    password: 'vendedor123',
    rol: 'Vendedor',
  },
  {
    id: 5,
    username: 'ccastro',
    password: 'tecnico123',
    rol: 'Técnico',
  },
];
