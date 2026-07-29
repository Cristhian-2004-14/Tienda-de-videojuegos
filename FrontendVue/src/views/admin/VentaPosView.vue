<script setup>
import { computed, onMounted, reactive, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import { storeToRefs } from 'pinia';
import AdminLayout from '../../components/AdminLayout.vue';
import ProductoVisual from '../../components/ProductoVisual.vue';
import PaginacionRegistros from '../../components/common/PaginacionRegistros.vue';
import SelectorCliente from '../../components/common/SelectorCliente.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { useAuthStore } from '../../stores/auth';
import { useNotificacionesStore } from '../../stores/notificaciones';

const router = useRouter();
const datos = useDatosApiStore();
const auth = useAuthStore();
const avisos = useNotificacionesStore();
const { productos, clientes } = storeToRefs(datos);

const lineas = ref([]);
const tipoCliente = ref('registrado');
const clienteId = ref('');
const clienteOcasional = reactive({
  nombre: '',
  apellido: '',
  ci: '',
  telefono: '',
  email: '',
  direccion: '',
});
const categoria = ref('Todos');
const busqueda = ref('');
const descuento = ref(0);
const error = ref('');
const guardando = ref(false);
const paginaProductos = ref(1);
const productosPorPagina = 5;
const categorias = ['Todos', 'Consolas', 'Videojuegos', 'Accesorios'];

const filtrados = computed(() => productos.value.filter((producto) =>
  (categoria.value === 'Todos' || producto.categoria === categoria.value)
  && (!busqueda.value || `${producto.nombre} ${producto.marca}`
    .toLowerCase().includes(busqueda.value.toLowerCase()))));
const productosPagina = computed(() => filtrados.value.slice(
  (paginaProductos.value - 1) * productosPorPagina,
  paginaProductos.value * productosPorPagina,
));
const subtotal = computed(() =>
  lineas.value.reduce((suma, linea) => suma + linea.precioUnitario * linea.cantidad, 0));
const total = computed(() => Math.max(0, subtotal.value - Number(descuento.value || 0)));
watch([categoria, busqueda], () => { paginaProductos.value = 1; });

function agregar(producto) {
  error.value = '';
  const linea = lineas.value.find((item) => item.productoId === producto.id);
  const cantidad = (linea?.cantidad || 0) + 1;
  if (cantidad > producto.stock) {
    error.value = `Stock insuficiente para ${producto.nombre}.`;
    return;
  }
  if (linea) linea.cantidad = cantidad;
  else {
    lineas.value.push({
      productoId: producto.id,
      producto: producto.nombre,
      edicion: 'Estándar',
      cantidad: 1,
      precioUnitario: producto.precioVenta,
    });
  }
}

function cambiar(linea, delta) {
  const producto = productos.value.find((item) => item.id === linea.productoId);
  const nuevaCantidad = linea.cantidad + delta;
  if (nuevaCantidad <= 0) {
    lineas.value = lineas.value.filter((item) => item !== linea);
    return;
  }
  if (!producto || nuevaCantidad > producto.stock) {
    error.value = `Solo hay ${producto?.stock || 0} unidades de ${linea.producto}.`;
    return;
  }
  linea.cantidad = nuevaCantidad;
}

function validarCliente() {
  if (tipoCliente.value === 'registrado') {
    if (!clienteId.value) throw new Error('Selecciona un cliente registrado.');
    return;
  }
  if (!clienteOcasional.nombre.trim()) throw new Error('Ingresa el nombre del cliente.');
  if (!clienteOcasional.ci.trim()) throw new Error('Ingresa el CI o NIT del cliente.');
  if (!clienteOcasional.telefono.trim()) throw new Error('Ingresa el teléfono del cliente.');
}

async function confirmar() {
  error.value = '';
  try {
    validarCliente();
    if (!lineas.value.length) throw new Error('Agrega al menos un producto.');
    const cliente = clientes.value.find((item) => item.id === Number(clienteId.value));
    const esRegistrado = tipoCliente.value === 'registrado';
    guardando.value = true;
    const venta = await datos.registrarVenta({
      clienteId: esRegistrado ? cliente.id : 0,
      cliente: esRegistrado
        ? `${cliente.nombre} ${cliente.apellido}`.trim()
        : `${clienteOcasional.nombre} ${clienteOcasional.apellido}`.trim(),
      tipoCliente: esRegistrado ? 'Registrado' : 'Ocasional',
      clienteCi: esRegistrado ? cliente.ci : clienteOcasional.ci,
      clienteTelefono: esRegistrado ? cliente.telefono : clienteOcasional.telefono,
      clienteEmail: esRegistrado ? cliente.email : clienteOcasional.email,
      clienteDireccion: esRegistrado ? cliente.direccion : clienteOcasional.direccion,
      empleadoId: auth.usuarioActual?.empleadoId || 1,
      empleado: auth.usuarioActual?.nombre || auth.usuarioActual?.username || 'Administrador',
      fecha: new Date().toISOString(),
      estado: 'Pendiente',
      descuento: Number(descuento.value || 0),
      detalles: lineas.value,
      pagos: [],
    });
    avisos.mostrar(`Venta #${venta.id} registrada. La factura está lista.`);
    router.push(`/admin/ventas/${venta.id}`);
  } catch (excepcion) {
    error.value = excepcion.response?.data?.message
      || excepcion.message
      || 'No se pudo registrar la venta.';
  } finally {
    guardando.value = false;
  }
}

function cancelar() {
  lineas.value = [];
  clienteId.value = '';
  Object.assign(clienteOcasional, {
    nombre: '', apellido: '', ci: '', telefono: '', email: '', direccion: '',
  });
  error.value = '';
}

onMounted(() => datos.cargarTodo());
</script>

<template>
  <AdminLayout titulo="Terminal de venta" buscador-placeholder="Buscar productos o SKU...">
    <template #header>
      <div class="encabezado">
        <div><p class="eyebrow mono">PUNTO DE VENTA</p><h2>Nueva venta</h2></div>
        <div class="turno"><span></span>Turno activo</div>
      </div>
    </template>

    <div class="pos-grid">
      <section>
        <div class="herramientas">
          <div class="busqueda">
            <span class="material-symbols-outlined">search</span>
            <input v-model="busqueda" placeholder="Buscar nombre o marca">
          </div>
          <div class="filtros">
            <button
              v-for="item in categorias"
              :key="item"
              :class="{ activo: categoria === item }"
              @click="categoria = item"
            >{{ item }}</button>
          </div>
        </div>
        <div class="productos">
          <button
            v-for="producto in productosPagina"
            :key="producto.id"
            class="producto"
            :disabled="producto.stock <= 0"
            @click="agregar(producto)"
          >
            <div class="visual"><ProductoVisual :producto="producto" /></div>
            <div class="producto-info">
              <p class="mono sku">SKU-{{ String(producto.id).padStart(4, '0') }}</p>
              <h3>{{ producto.nombre }}</h3>
              <p>{{ producto.marca }}</p>
              <div>
                <strong>${{ producto.precioVenta.toFixed(2) }}</strong>
                <span :class="{ agotado: producto.stock <= 0 }">
                  {{ producto.stock }} disponibles
                </span>
              </div>
            </div>
          </button>
        </div>
        <PaginacionRegistros
          v-model:pagina="paginaProductos"
          :total="filtrados.length"
          :por-pagina="productosPorPagina"
        />
      </section>

      <aside class="orden">
        <div class="orden-titulo">
          <div>
            <p class="mono">RESUMEN DE VENTA</p>
            <h3>Orden actual</h3>
          </div>
          <span class="contador">{{ lineas.length }} {{ lineas.length === 1 ? 'línea' : 'líneas' }}</span>
        </div>

        <section class="bloque-cliente">
          <div class="titulo-bloque">
            <span class="material-symbols-outlined">person</span>
            <div><strong>Cliente</strong><small>Selecciona cómo emitir la factura</small></div>
          </div>
        <div class="tipo-cliente" role="group" aria-label="Tipo de cliente">
          <button
            type="button"
            :class="{ activo: tipoCliente === 'registrado' }"
            :aria-pressed="tipoCliente === 'registrado'"
            @click="tipoCliente = 'registrado'"
          >Cliente registrado</button>
          <button
            type="button"
            :class="{ activo: tipoCliente === 'ocasional' }"
            :aria-pressed="tipoCliente === 'ocasional'"
            @click="tipoCliente = 'ocasional'"
          >Cliente ocasional</button>
        </div>

        <SelectorCliente
          v-if="tipoCliente === 'registrado'"
          v-model="clienteId"
          :clientes="clientes"
          label="Cliente registrado"
        />

        <div v-else class="cliente-ocasional">
          <div class="encabezado-formulario">
            <strong>Datos para la factura</strong>
            <small>Los campos con * son obligatorios</small>
          </div>
          <div class="campos-cliente">
            <div class="campo-cliente"><label for="ocasional-nombre">Nombre *</label><input id="ocasional-nombre" v-model.trim="clienteOcasional.nombre" autocomplete="given-name"></div>
            <div class="campo-cliente"><label for="ocasional-apellido">Apellido</label><input id="ocasional-apellido" v-model.trim="clienteOcasional.apellido" autocomplete="family-name"></div>
            <div class="campo-cliente"><label for="ocasional-ci">CI o NIT *</label><input id="ocasional-ci" v-model.trim="clienteOcasional.ci"></div>
            <div class="campo-cliente"><label for="ocasional-telefono">Teléfono *</label><input id="ocasional-telefono" v-model.trim="clienteOcasional.telefono" type="tel" autocomplete="tel"></div>
            <div class="campo-cliente ancho"><label for="ocasional-email">Correo electrónico</label><input id="ocasional-email" v-model.trim="clienteOcasional.email" type="email" autocomplete="email"></div>
            <div class="campo-cliente ancho"><label for="ocasional-direccion">Dirección</label><textarea id="ocasional-direccion" v-model.trim="clienteOcasional.direccion" rows="2" autocomplete="street-address"></textarea></div>
          </div>
          <p class="ayuda"><span class="material-symbols-outlined">info</span>Estos datos se guardan únicamente dentro de esta venta.</p>
        </div>
        </section>

        <section class="bloque-productos">
          <div class="titulo-bloque">
            <span class="material-symbols-outlined">shopping_cart</span>
            <div><strong>Productos</strong><small>Artículos incluidos en la venta</small></div>
          </div>
        <div v-if="lineas.length" class="lineas">
          <article v-for="linea in lineas" :key="linea.productoId">
            <div><strong>{{ linea.producto }}</strong><small>${{ linea.precioUnitario.toFixed(2) }} c/u</small></div>
            <b>${{ (linea.precioUnitario * linea.cantidad).toFixed(2) }}</b>
            <div class="cantidad">
              <button type="button" :aria-label="`Quitar una unidad de ${linea.producto}`" @click="cambiar(linea, -1)">−</button>
              <span>{{ linea.cantidad }}</span>
              <button type="button" :aria-label="`Agregar una unidad de ${linea.producto}`" @click="cambiar(linea, 1)">+</button>
            </div>
          </article>
        </div>
        <div v-else class="vacio">
          <span class="material-symbols-outlined">add_shopping_cart</span>
          <strong>La orden está vacía</strong>
          <p>Selecciona productos del catálogo para comenzar.</p>
        </div>
        </section>

        <section class="totales">
          <p><span>Subtotal</span><strong>${{ subtotal.toFixed(2) }}</strong></p>
          <label class="descuento">
            <span>Descuento</span>
            <input v-model.number="descuento" type="number" min="0" :max="subtotal" step=".01">
          </label>
          <p class="total"><span>Total</span><strong>${{ total.toFixed(2) }}</strong></p>
        </section>
        <p v-if="error" class="error" role="alert">{{ error }}</p>
        <button class="cobrar" :disabled="guardando || !lineas.length" @click="confirmar">
          <span class="material-symbols-outlined">receipt_long</span>
          {{ guardando ? 'Registrando...' : 'Registrar venta y generar factura' }}
        </button>
        <button type="button" class="cancelar" @click="cancelar">Limpiar orden</button>
      </aside>
    </div>
  </AdminLayout>
</template>

<style scoped>
.encabezado{display:flex;justify-content:space-between;align-items:end}.encabezado h2{font-size:32px}.eyebrow{font-size:11px;color:var(--color-primary)}.turno{display:flex;gap:8px;align-items:center;color:#aab4a6}.turno span{width:8px;height:8px;border-radius:50%;background:var(--color-primary);box-shadow:0 0 0 5px #79dd6812}.pos-grid{display:grid;grid-template-columns:minmax(0,1fr) minmax(390px,430px);gap:28px;align-items:start}.herramientas{display:flex;gap:12px;justify-content:space-between;margin-bottom:20px}.busqueda{display:flex;align-items:center;gap:8px;min-width:280px;background:var(--color-surface-container-low);border:1px solid #333;padding:10px 14px;border-radius:8px}.busqueda input{width:100%;border:0;background:transparent;color:#fff;outline:0}.filtros{display:flex;gap:7px}.filtros button{min-height:44px;border:1px solid #303030;background:#1b1b1b;color:#bbb;padding:9px 13px;border-radius:7px;cursor:pointer}.filtros .activo{border-color:#159514;background:#107c10;color:#fff}.productos{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:16px}.producto{display:grid;grid-template-columns:125px 1fr;min-height:176px;padding:0;text-align:left;border:1px solid #292929;background:#1b1b1b;color:#fff;border-radius:9px;overflow:hidden;cursor:pointer;transition:border-color .2s,background .2s}.producto:hover{border-color:#79dd68;background:#1e211e}.producto:focus-visible{outline:2px solid var(--color-primary);outline-offset:2px}.producto:disabled{opacity:.42;cursor:not-allowed}.visual{min-width:0;padding:12px;background:#242424}.visual :deep(.visual){min-height:150px}.producto-info{min-width:0;padding:16px;display:flex;flex-direction:column}.sku{font-size:9px;color:var(--color-primary)}.producto h3{font-size:16px;margin:5px 0;overflow-wrap:anywhere}.producto-info>p:not(.sku){color:#999;font-size:11px}.producto-info>div{margin-top:auto;display:flex;justify-content:space-between;align-items:end;gap:10px}.producto-info span{font-size:9px;color:#9eaa9a}.agotado{color:#ffb4ab!important}
.orden{width:100%;overflow:hidden;background:var(--color-surface-container-low);border:1px solid #2d322d;border-left:3px solid #107c10;border-radius:10px;height:max-content;position:sticky;top:88px}.orden-titulo{display:flex;justify-content:space-between;align-items:center;padding:20px 22px;border-bottom:1px solid #2e332e}.orden-titulo p{font-size:9px;color:var(--color-primary);letter-spacing:.1em}.orden-titulo h3{margin-top:3px;font-size:20px}.contador{padding:5px 9px;border:1px solid #3a4239;border-radius:999px;color:#aeb7aa;font-size:10px}.bloque-cliente,.bloque-productos{padding:18px 22px;border-bottom:1px solid #2e332e}.titulo-bloque{display:flex;align-items:center;gap:10px;margin-bottom:14px}.titulo-bloque>.material-symbols-outlined{width:34px;height:34px;display:grid;place-items:center;border-radius:7px;background:#213021;color:var(--color-primary);font-size:18px}.titulo-bloque div{display:flex;flex-direction:column}.titulo-bloque strong{font-size:13px}.titulo-bloque small{margin-top:2px;color:#7f897d;font-size:10px}.tipo-cliente{display:grid;grid-template-columns:1fr 1fr;gap:4px;margin-bottom:14px;padding:4px;background:#101210;border:1px solid #282d28;border-radius:8px}.tipo-cliente button{min-height:40px;border:0;border-radius:6px;padding:8px;background:transparent;color:#929a90;font-size:11px;cursor:pointer}.tipo-cliente button:hover{color:#fff}.tipo-cliente .activo{background:#107c10;color:#fff;box-shadow:0 3px 12px #0005}.selector-cliente{gap:7px}.selector-cliente select{min-height:44px}.cliente-ocasional{display:grid;gap:12px}.encabezado-formulario{display:flex;justify-content:space-between;align-items:center;gap:12px}.encabezado-formulario strong{font-size:12px}.encabezado-formulario small{color:#788277;font-size:9px}.campos-cliente{display:grid;grid-template-columns:minmax(0,1fr) minmax(0,1fr);gap:12px}.campo-cliente{min-width:0;display:flex;flex-direction:column;gap:6px}.campo-cliente label{display:block;color:#b9c2b5;font-size:9px;font-weight:700;letter-spacing:.04em;text-transform:uppercase}.campo-cliente input,.campo-cliente textarea{display:block;width:100%;min-width:0;min-height:42px;border:1px solid #343a34;border-radius:6px;background:#111411;color:#f1f3f0;padding:10px 11px;font:inherit;outline:none}.campo-cliente textarea{min-height:64px;resize:vertical}.campo-cliente input:focus,.campo-cliente textarea:focus{border-color:var(--color-primary);box-shadow:0 0 0 3px #79dd6812}.campo-cliente.ancho{grid-column:1/-1}.ayuda{display:flex;align-items:flex-start;gap:6px;color:#7f897d;font-size:9px;line-height:1.45}.ayuda .material-symbols-outlined{font-size:14px;color:#9da79a}.lineas article{display:grid;grid-template-columns:minmax(0,1fr) auto;gap:9px;padding:13px 0;border-bottom:1px solid #303530}.lineas article:last-child{border-bottom:0}.lineas strong{font-size:12px}.lineas small{display:block;margin-top:3px;color:#8d978a;font-size:10px}.lineas b{font-size:12px;font-variant-numeric:tabular-nums}.cantidad{display:flex;align-items:center;gap:5px}.cantidad button,.cantidad span{width:32px;height:32px;display:grid;place-items:center}.cantidad button{border:1px solid #383e38;border-radius:5px;background:#252925;color:#fff;cursor:pointer}.cantidad button:hover{border-color:var(--color-primary);color:var(--color-primary)}.cantidad span{font-size:11px}.vacio{display:grid;justify-items:center;gap:6px;padding:26px 12px;color:#768075;text-align:center}.vacio>.material-symbols-outlined{font-size:30px;color:#526050}.vacio strong{color:#aeb7aa;font-size:12px}.vacio p{font-size:10px}.totales{display:grid;gap:10px;padding:18px 22px}.totales p,.descuento{display:flex;align-items:center;justify-content:space-between;color:#aab3a7;font-size:12px}.totales strong{font-variant-numeric:tabular-nums;color:#fff}.descuento input{width:105px;min-height:38px;border:1px solid #343a34;border-radius:6px;background:#111411;color:#fff;padding:8px 10px;text-align:right}.totales .total{margin-top:3px;padding-top:14px;border-top:1px solid #343934;font-size:18px}.totales .total strong{color:var(--color-primary);font-size:23px}.error{margin:0 22px 14px;padding:11px;border:1px solid #633;background:#2c1717;color:#ffb4ab;border-radius:6px;font-size:11px}.cobrar,.cancelar{width:calc(100% - 44px);min-height:46px;margin-inline:22px;border:0;border-radius:7px;font-weight:800;cursor:pointer}.cobrar{display:flex;align-items:center;justify-content:center;gap:8px;background:#107c10;color:#fff}.cobrar:hover:not(:disabled){background:#159514}.cobrar:disabled{opacity:.42;cursor:not-allowed}.cancelar{margin-top:8px;margin-bottom:16px;background:transparent;color:#899285}.cancelar:hover{background:#242824;color:#fff}
@media(max-width:1180px){.pos-grid{grid-template-columns:1fr}.orden{position:static;max-width:700px}.productos{grid-template-columns:repeat(2,minmax(0,1fr))}}@media(max-width:760px){.encabezado{align-items:flex-start;flex-direction:column}.productos{grid-template-columns:1fr}.herramientas{flex-direction:column}.busqueda{min-width:0}.filtros{overflow:auto;padding-bottom:4px}.filtros button{white-space:nowrap}}@media(max-width:480px){.campos-cliente{grid-template-columns:1fr}.campo-cliente.ancho{grid-column:auto}.orden-titulo,.bloque-cliente,.bloque-productos,.totales{padding-inline:16px}.cobrar,.cancelar{width:calc(100% - 32px);margin-inline:16px}}
</style>
