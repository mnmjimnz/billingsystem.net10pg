# Reglas de Interfaz y UI (Establecidas por el Usuario)

1. **PROHIBIDO EL USO DE NATIVE ALERTS**:
   - Nunca utilices `alert()`, `confirm()`, ni `prompt()` nativos del navegador.
   - Todo mensaje de confirmación debe utilizar ventanas modales personalizadas de Bootstrap (por ejemplo, `confirmStatusModal`).
   - Todos los mensajes de éxito/error deben mostrarse usando el sistema de notificaciones/toasts incorporado (`showToast()`).

2. **CODIFICACIÓN ESTRICTA EN UTF-8**:
   - Para evitar la corrupción de caracteres especiales y letras con tilde (problema de los "diamantes con signos de interrogación"), todo archivo editado o generado DEBE guardarse estrictamente en formato UTF-8.
   - Si se usa PowerShell para escribir archivos (por ejemplo `Set-Content`), **siempre** debes añadir la bandera `-Encoding UTF8`. Alternativamente, utiliza las herramientas nativas del entorno (`write_to_file` / `replace_file_content`) que ya manejan la codificación de manera segura.

