<script setup>
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'

const props = defineProps({
  modelValue: {
    type: [String, Number],
    default: '',
  },
  clientes: {
    type: Array,
    default: () => [],
  },
  label: {
    type: String,
    default: 'Cliente',
  },
  placeholder: {
    type: String,
    default: 'Buscar por nombre, CI o teléfono',
  },
})

const emit = defineEmits(['update:modelValue', 'change'])

const contenedor = ref(null)
const busqueda = ref('')
const abierto = ref(false)

const clienteSeleccionado = computed(() =>
  props.clientes.find((cliente) => String(cliente.id) === String(props.modelValue)),
)

const clientesFiltrados = computed(() => {
  const termino = busqueda.value.trim().toLocaleLowerCase('es')
  const resultados = termino
    ? props.clientes.filter((cliente) => {
        const datos = [
          cliente.nombre,
          cliente.apellido,
          cliente.ci,
          cliente.telefono,
          cliente.email,
          cliente.correo,
        ]
          .filter(Boolean)
          .join(' ')
          .toLocaleLowerCase('es')

        return datos.includes(termino)
      })
    : props.clientes

  return resultados.slice(0, 7)
})

function nombreCompleto(cliente) {
  return [cliente?.nombre, cliente?.apellido].filter(Boolean).join(' ') || 'Cliente sin nombre'
}

function seleccionar(cliente) {
  emit('update:modelValue', cliente.id)
  emit('change', cliente)
  busqueda.value = ''
  abierto.value = false
}

function quitarSeleccion() {
  emit('update:modelValue', '')
  emit('change', null)
  busqueda.value = ''
  abierto.value = true
}

function cerrarAlHacerClickAfuera(evento) {
  if (!contenedor.value?.contains(evento.target)) abierto.value = false
}

onMounted(() => document.addEventListener('pointerdown', cerrarAlHacerClickAfuera))
onBeforeUnmount(() => document.removeEventListener('pointerdown', cerrarAlHacerClickAfuera))
</script>

<template>
  <div ref="contenedor" class="selector-cliente">
    <label class="selector-cliente__label">{{ label }}</label>

    <div v-if="clienteSeleccionado" class="selector-cliente__seleccion">
      <div class="selector-cliente__avatar" aria-hidden="true">
        {{ nombreCompleto(clienteSeleccionado).charAt(0).toUpperCase() }}
      </div>
      <div class="selector-cliente__datos">
        <strong>{{ nombreCompleto(clienteSeleccionado) }}</strong>
        <span>
          {{ clienteSeleccionado.ci ? `CI ${clienteSeleccionado.ci}` : 'Sin CI' }}
          <template v-if="clienteSeleccionado.telefono"> · {{ clienteSeleccionado.telefono }}</template>
        </span>
      </div>
      <button
        type="button"
        class="selector-cliente__quitar"
        aria-label="Quitar cliente seleccionado"
        title="Cambiar cliente"
        @click="quitarSeleccion"
      >
        ×
      </button>
    </div>

    <div class="selector-cliente__buscador" :class="{ activo: abierto }">
      <span class="selector-cliente__lupa" aria-hidden="true">⌕</span>
      <input
        v-model="busqueda"
        type="search"
        :placeholder="clienteSeleccionado ? 'Buscar otro cliente' : placeholder"
        autocomplete="off"
        @focus="abierto = true"
        @keydown.escape="abierto = false"
      />
    </div>

    <div v-if="abierto" class="selector-cliente__resultados" role="listbox">
      <button
        v-for="cliente in clientesFiltrados"
        :key="cliente.id"
        type="button"
        class="selector-cliente__opcion"
        :class="{ seleccionado: String(cliente.id) === String(modelValue) }"
        role="option"
        :aria-selected="String(cliente.id) === String(modelValue)"
        @click="seleccionar(cliente)"
      >
        <span>
          <strong>{{ nombreCompleto(cliente) }}</strong>
          <small>
            {{ cliente.ci ? `CI ${cliente.ci}` : 'Sin CI' }}
            <template v-if="cliente.telefono"> · {{ cliente.telefono }}</template>
          </small>
        </span>
        <span class="selector-cliente__accion">
          {{ String(cliente.id) === String(modelValue) ? 'Seleccionado' : 'Elegir' }}
        </span>
      </button>

      <div v-if="clientesFiltrados.length === 0" class="selector-cliente__vacio">
        No se encontraron clientes con esos datos.
      </div>
    </div>
  </div>
</template>

<style scoped>
.selector-cliente {
  position: relative;
  display: grid;
  gap: 0.55rem;
}

.selector-cliente__label {
  color: var(--color-texto-secundario, #c7d0c2);
  font-size: 0.78rem;
  font-weight: 800;
  letter-spacing: 0.06em;
  text-transform: uppercase;
}

.selector-cliente__seleccion {
  display: flex;
  align-items: center;
  gap: 0.7rem;
  min-height: 3.25rem;
  padding: 0.65rem 0.75rem;
  border: 1px solid rgba(105, 222, 91, 0.42);
  border-radius: 0.75rem;
  background: rgba(34, 108, 37, 0.16);
}

.selector-cliente__avatar {
  display: grid;
  flex: 0 0 2rem;
  width: 2rem;
  height: 2rem;
  place-items: center;
  border-radius: 50%;
  color: #0d160d;
  font-weight: 900;
  background: #69de5b;
}

.selector-cliente__datos {
  display: grid;
  min-width: 0;
  gap: 0.1rem;
}

.selector-cliente__datos strong {
  overflow: hidden;
  color: #f4f6f3;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.selector-cliente__datos span {
  overflow: hidden;
  color: #aab2a6;
  font-size: 0.78rem;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.selector-cliente__quitar {
  display: grid;
  width: 2rem;
  height: 2rem;
  margin-left: auto;
  border: 0;
  border-radius: 50%;
  place-items: center;
  color: #cdd3ca;
  font-size: 1.35rem;
  cursor: pointer;
  background: rgba(255, 255, 255, 0.06);
}

.selector-cliente__quitar:hover {
  color: #fff;
  background: rgba(255, 255, 255, 0.12);
}

.selector-cliente__buscador {
  display: flex;
  align-items: center;
  min-height: 2.85rem;
  overflow: hidden;
  border: 1px solid #343734;
  border-radius: 0.7rem;
  background: #101110;
  transition: border-color 0.18s ease, box-shadow 0.18s ease;
}

.selector-cliente__buscador.activo {
  border-color: #69de5b;
  box-shadow: 0 0 0 3px rgba(105, 222, 91, 0.1);
}

.selector-cliente__lupa {
  padding-left: 0.85rem;
  color: #69de5b;
  font-size: 1.25rem;
}

.selector-cliente__buscador input {
  width: 100%;
  min-width: 0;
  padding: 0.72rem 0.85rem;
  border: 0;
  outline: 0;
  color: #f3f5f2;
  font: inherit;
  background: transparent;
}

.selector-cliente__buscador input::placeholder {
  color: #777e75;
}

.selector-cliente__resultados {
  position: absolute;
  z-index: 30;
  top: calc(100% + 0.4rem);
  right: 0;
  left: 0;
  max-height: 18rem;
  padding: 0.35rem;
  overflow-y: auto;
  border: 1px solid #383b38;
  border-radius: 0.75rem;
  background: #171917;
  box-shadow: 0 1rem 2.5rem rgba(0, 0, 0, 0.45);
}

.selector-cliente__opcion {
  display: flex;
  width: 100%;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  padding: 0.7rem 0.75rem;
  border: 0;
  border-radius: 0.55rem;
  color: #eff2ed;
  text-align: left;
  cursor: pointer;
  background: transparent;
}

.selector-cliente__opcion:hover,
.selector-cliente__opcion:focus-visible,
.selector-cliente__opcion.seleccionado {
  outline: none;
  background: rgba(105, 222, 91, 0.11);
}

.selector-cliente__opcion > span:first-child {
  display: grid;
  min-width: 0;
  gap: 0.15rem;
}

.selector-cliente__opcion strong,
.selector-cliente__opcion small {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.selector-cliente__opcion small {
  color: #969e93;
  font-size: 0.75rem;
}

.selector-cliente__accion {
  color: #69de5b;
  font-size: 0.72rem;
  font-weight: 800;
}

.selector-cliente__vacio {
  padding: 1rem;
  color: #9da49a;
  font-size: 0.85rem;
  text-align: center;
}
</style>
