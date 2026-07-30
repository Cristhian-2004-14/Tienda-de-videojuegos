<script setup>
import { computed, onMounted, ref, watch } from 'vue';
import { storeToRefs } from 'pinia';
import TiendaLayout from '../../components/TiendaLayout.vue';
import ProductoVisual from '../../components/ProductoVisual.vue';
import PaginacionRegistros from '../../components/common/PaginacionRegistros.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { productosApi } from '../../services/recursosApi';
import { useCarritoStore } from '../../stores/carrito';
import { useTiendaUiStore } from '../../stores/tiendaUi';

const datosStore = useDatosApiStore();
const carritoStore = useCarritoStore();
const uiStore = useTiendaUiStore();
const { productos } = storeToRefs(datosStore);
const { busqueda, categoria } = storeToRefs(uiStore);
const hero = ref(null);
const pagina = ref(1);
const porPagina = 8;
const inicioEscena = Math.floor(Math.random() * 10000);
const videojuegosEscena = computed(() => {
  const juegosConImagen = productos.value
    .filter((producto) => producto.categoria === 'Videojuegos')
    .filter((producto) => producto.imagenes?.[0]?.url || producto.imagenUrl);
  if (juegosConImagen.length <= 16) return juegosConImagen;

  const inicio = inicioEscena % juegosConImagen.length;
  return [...juegosConImagen.slice(inicio), ...juegosConImagen.slice(0, inicio)].slice(0, 16);
});
const productosFiltrados = computed(() => {
  const termino = busqueda.value.trim().toLowerCase();
  return productos.value
    .filter((producto) => categoria.value === 'Todo' || producto.categoria === categoria.value)
    .filter((producto) => !termino || [producto.nombre, producto.marca, producto.categoria]
      .some((valor) => valor.toLowerCase().includes(termino)));
});
const productosPagina = computed(() => productosFiltrados.value.slice(
  (pagina.value - 1) * porPagina,
  pagina.value * porPagina,
));

function moverEscena(evento) {
  if (!hero.value) return;
  const rect = hero.value.getBoundingClientRect();
  hero.value.style.setProperty('--parallax-x', `${((evento.clientX - rect.left) / rect.width - 0.5) * 22}px`);
  hero.value.style.setProperty('--parallax-y', `${((evento.clientY - rect.top) / rect.height - 0.5) * 16}px`);
}

watch([busqueda, categoria], () => { pagina.value = 1; });
onMounted(async () => {
  await datosStore.cargarRecurso('productos', productosApi);
});
</script>

<template>
  <TiendaLayout>
    <div v-if="videojuegosEscena.length" class="fondo-videojuegos" aria-hidden="true">
      <div v-for="columna in 5" :key="columna" class="columna-videojuegos" :class="`columna-${columna}`">
        <div class="cinta-vertical">
          <div v-for="repeticion in 3" :key="repeticion" class="grupo-vertical">
            <div v-for="juego in videojuegosEscena" :key="`${columna}-${repeticion}-${juego.id}`" class="portada-fondo">
              <ProductoVisual :producto="juego" />
            </div>
          </div>
        </div>
      </div>
    </div>
    <section ref="hero" class="hero hero-estatico" @pointermove="moverEscena" @pointerleave="hero?.style.removeProperty('--parallax-x');hero?.style.removeProperty('--parallax-y')">
      <div class="muro-productos" aria-hidden="true">
        <div v-for="fila in 3" :key="fila" class="fila-caratulas" :class="`fila-${fila}`">
          <div class="cinta-caratulas">
            <div v-for="repeticion in 3" :key="repeticion" class="grupo-caratulas">
              <div v-for="producto in videojuegosEscena" :key="`${fila}-${repeticion}-${producto.id}`" class="caratula">
                <ProductoVisual :producto="producto" />
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="hero-ruido"></div>
      <div class="hero-copy">
        <p class="tag">X-STORE · EQUIPA TU MUNDO</p>
        <h1><span>Juega.</span><br>Sin límites.</h1>
        <p>Consolas, videojuegos y accesorios reunidos en un catálogo hecho para encontrar tu próxima experiencia.</p>
        <div class="hero-acciones">
          <a href="#productos">Explorar catálogo <span>↓</span></a>
          <button @click="uiStore.seleccionarCategoria('Consolas'); $nextTick(() => document.querySelector('#productos')?.scrollIntoView({behavior:'smooth'}))">Ver hardware</button>
        </div>
      </div>
      <div class="hero-sello mono"><span>CATÁLOGO</span><strong>{{ productos.length }}</strong><small>PRODUCTOS</small></div>
      <p class="hero-scroll mono">DESLIZA PARA EXPLORAR <span></span></p>
    </section>
    <main id="productos" class="catalogo">
      <div class="titulo"><div><p class="sobre">EXPLORA EL CATÁLOGO</p><h2>{{ busqueda ? `Resultados para “${busqueda}”` : 'Encuentra tu próxima aventura' }}</h2></div><p>{{ productosFiltrados.length }} productos</p></div>
      <div class="filtros"><button v-for="filtro in ['Todo','Consolas','Videojuegos','Accesorios']" :key="filtro" :class="{ activo: categoria === filtro }" @click="uiStore.seleccionarCategoria(filtro)">{{ filtro }}</button><span></span><select aria-label="Ordenar"><option>Más destacados</option><option>Precio: menor a mayor</option></select></div>
      <div class="grid">
        <router-link v-for="producto in productosPagina" :key="producto.id" :to="`/tienda/producto/${producto.id}`" class="card">
          <div class="media-card"><span v-if="producto.stock < 5 && producto.stock > 0" class="alerta">ÚLTIMAS UNIDADES</span><ProductoVisual :producto="producto" /></div>
          <div class="info"><p>{{ producto.categoria }} · {{ producto.marca }}</p><h3>{{ producto.nombre }}</h3><div><strong>${{ producto.precioVenta.toFixed(2) }}</strong><button aria-label="Agregar a la selección" :disabled="producto.stock <= 0" @click.prevent="carritoStore.agregarProducto(producto)"><span class="material-symbols-outlined">add_shopping_cart</span></button></div></div>
        </router-link>
      </div>
      <PaginacionRegistros v-model:pagina="pagina" :total="productosFiltrados.length" :por-pagina="porPagina" />
      <div v-if="!productosFiltrados.length" class="sin-resultados"><span class="material-symbols-outlined">search_off</span><h3>No encontramos productos</h3><p>Prueba otra búsqueda o cambia la categoría.</p><button @click="busqueda = ''; uiStore.seleccionarCategoria('Todo')">Mostrar todo</button></div>
    </main>
  </TiendaLayout>
</template>

<style scoped>
.hero{min-height:560px;position:relative;overflow:hidden;display:grid;grid-template-columns:1.08fr .92fr;align-items:center;padding:55px clamp(25px,7vw,110px);background:radial-gradient(circle at 78% 40%,#1d531f 0,#122b14 20%,#101010 58%)}.hero:after{content:"";position:absolute;inset:0;background:repeating-linear-gradient(90deg,transparent 0,transparent calc(8.33% - 1px),#ffffff08 calc(8.33% - 1px),#ffffff08 8.33%);pointer-events:none}.hero-copy{z-index:2;max-width:650px}.tag,.sobre{color:#79dd68;font-size:11px;font-weight:900;letter-spacing:.16em}.hero h1{font-size:clamp(46px,7vw,82px);line-height:.94;letter-spacing:-.06em;margin:15px 0 22px}.hero-copy>p:not(.tag){color:#c7cec4;line-height:1.65;max-width:520px}.hero-acciones{display:flex;gap:12px;flex-wrap:wrap;margin-top:27px}.hero-acciones a,.hero-acciones button{min-height:48px;display:inline-flex;gap:20px;align-items:center;border:0;border-radius:5px;padding:14px 20px;font-weight:800}.hero-acciones a{background:#107c10}.hero-acciones button{background:#252525;color:#fff}.hero-product{z-index:1;display:grid;place-items:center;transform:scale(2.15);position:relative}.hero-product strong{position:absolute;transform:scale(.46);bottom:-38px;background:#0d0d0dd9;border:1px solid #79dd68;padding:10px 18px;color:#79dd68;font-size:20px}.halo{position:absolute;width:240px;height:240px;border-radius:50%;background:#79dd6840;filter:blur(35px)}.hero-index{position:absolute;bottom:28px;right:7vw;color:#8b9788;font-size:11px}.controles{position:absolute;z-index:3;bottom:24px;left:clamp(25px,7vw,110px);display:flex;align-items:center;gap:13px}.controles>button{width:36px;height:36px;border:1px solid #ffffff28;background:#171717;color:#fff}.controles>div{display:flex;gap:6px}.controles>div button{width:24px;height:4px;padding:0;border:0;background:#ffffff35}.controles>div button.activo{background:#79dd68}.hero-vacio{grid-template-columns:1fr}.catalogo{max-width:1440px;margin:auto;padding:68px clamp(20px,4vw,54px)}.titulo{display:flex;justify-content:space-between;align-items:end}.titulo h2{font-size:clamp(27px,4vw,40px);margin-top:10px}.titulo>p{color:#8d958a;font-size:12px}.filtros{display:flex;gap:10px;margin:30px 0;align-items:center;border-bottom:1px solid #292929;padding-bottom:18px}.filtros button,.filtros select{border:0;background:transparent;color:#a2a2a2;padding:10px 14px}.filtros button.activo{background:#107c10;color:#fff;border-radius:5px}.filtros span{flex:1}.filtros select{background:#1b1b1b;border-radius:5px}.grid{display:grid;grid-template-columns:repeat(3,1fr);gap:20px}.card{min-height:390px;position:relative;background:#191919;border:1px solid #ffffff10;border-radius:12px;overflow:hidden;isolation:isolate;transition:transform .3s ease,border-color .3s ease,box-shadow .3s ease}.card:hover{transform:translateY(-6px);border-color:#79dd6866;box-shadow:0 22px 55px #0009}.media-card{position:absolute;inset:0;background:#222}.media-card:after{content:"";position:absolute;inset:0;z-index:2;background:linear-gradient(180deg,transparent 35%,rgba(7,9,7,.18) 53%,rgba(7,9,7,.97) 84%);pointer-events:none}.media-card :deep(.visual){width:100%;height:100%;min-height:0}.alerta{position:absolute;z-index:4;top:14px;left:14px;background:#79dd68;color:#053706;padding:6px 9px;border-radius:4px;font-size:9px;font-weight:900}.info{position:absolute;z-index:3;inset:auto 0 0;padding:24px}.info>p{font-size:10px;color:#9ff18f;text-transform:uppercase}.info h3{font-size:21px;line-height:1.15;margin:8px 0 18px;text-shadow:0 2px 12px #000}.info>div{display:flex;align-items:center;justify-content:space-between}.info strong{font-size:21px}.info button{display:grid;place-items:center;width:42px;height:42px;border:1px solid #ffffff26;border-radius:50%;background:#107c10;color:#fff}.info button:hover{background:#79dd68;color:#092509}
@media(max-width:850px){.hero{grid-template-columns:1fr;min-height:570px}.hero-product{opacity:.25;position:absolute;right:10%;transform:scale(2.7)}.grid{grid-template-columns:repeat(2,1fr)}}@media(max-width:560px){.grid{grid-template-columns:1fr}.titulo{align-items:start;gap:10px;flex-direction:column}.filtros{overflow:auto}.filtros span{display:none}}
.sin-resultados{text-align:center;padding:70px 20px;background:#191919;border:1px solid #2d2d2d;border-radius:8px}.sin-resultados>span{font-size:45px;color:#79dd68}.sin-resultados h3{font-size:22px;margin:12px 0 7px}.sin-resultados p{color:#8d958a;margin-bottom:20px}.sin-resultados button{border:0;background:#107c10;color:#fff;padding:12px 18px;border-radius:5px;font-weight:800}

/* Hero estático: el desenfoque de las carátulas es intencional y evita
   ampliar imágenes comprimidas como si fueran fotografía principal. */
.hero-estatico{--parallax-x:0px;--parallax-y:0px;min-height:680px;display:flex;align-items:center;isolation:isolate;overflow:hidden;background:#080a08;padding:80px clamp(25px,7vw,110px)}
.hero-estatico:before{content:"";position:absolute;inset:0;z-index:0;background:radial-gradient(circle at 72% 38%,#2c762d70 0,transparent 28%),radial-gradient(circle at 18% 88%,#143f2380 0,transparent 34%),linear-gradient(110deg,#080a08 15%,#101910 55%,#070807)}
.muro-productos{position:absolute;inset:-18% -10%;z-index:1;display:grid;align-content:center;gap:18px;transform:translate3d(var(--parallax-x),var(--parallax-y),0) rotate(-8deg) scale(1.08);transition:transform .8s cubic-bezier(.2,.75,.2,1);filter:saturate(.82);opacity:.76;overflow:hidden}
.fila-caratulas{display:flex;width:100%;overflow:hidden;mask-image:linear-gradient(90deg,transparent 0,#000 12%,#000 91%,transparent)}
.cinta-caratulas,.grupo-caratulas{display:flex;flex:none;gap:18px}.cinta-caratulas{width:max-content;animation:cascadaSteam 34s linear infinite;will-change:transform}.fila-2 .cinta-caratulas{animation-name:cascadaSteamInversa;animation-duration:40s;transform:translateX(-32%)}.fila-3 .cinta-caratulas{animation-duration:46s}
.caratula{flex:none;width:clamp(145px,13vw,210px);aspect-ratio:16/10;overflow:hidden;border:1px solid #ffffff24;border-radius:8px;background:#111;box-shadow:0 20px 45px #000c;transform:translateZ(0)}
.caratula :deep(.visual){min-height:0}.caratula :deep(.visual img){filter:blur(1.2px) contrast(1.08);transform:scale(1.04)}.caratula :deep(.visual small){display:none}
.hero-ruido{position:absolute;inset:0;z-index:2;pointer-events:none;background:linear-gradient(90deg,#080a08f7 0,#080a08d9 38%,#080a0855 70%,#080a08a8 100%),repeating-linear-gradient(0deg,#ffffff08 0 1px,transparent 1px 4px)}
.hero-estatico .hero-copy{max-width:760px;z-index:4}.hero-estatico .tag{display:flex;align-items:center;gap:12px;font-size:10px;letter-spacing:.25em}.hero-estatico .tag:before{content:"";width:42px;height:1px;background:#79dd68}
.hero-estatico h1{margin:20px 0 26px;font-size:clamp(72px,10vw,142px);line-height:.75;letter-spacing:-.075em;text-transform:uppercase;text-shadow:0 8px 45px #000}.hero-estatico h1 span{color:transparent;-webkit-text-stroke:1.5px #d9e6d7}
.hero-estatico .hero-copy>p:not(.tag){max-width:580px;color:#c0c9bd;font-size:clamp(16px,1.5vw,20px);line-height:1.65}
.hero-estatico .hero-acciones{margin-top:34px}.hero-estatico .hero-acciones a,.hero-estatico .hero-acciones button{min-height:54px;padding:15px 24px;border-radius:2px}.hero-estatico .hero-acciones a{background:#79dd68;color:#092308}.hero-estatico .hero-acciones button{border:1px solid #ffffff40;background:#0d100dcc;backdrop-filter:blur(8px)}
.hero-sello{position:absolute;z-index:4;right:clamp(25px,6vw,90px);bottom:72px;width:126px;height:126px;display:grid;place-content:center;text-align:center;border:1px solid #79dd6875;border-radius:50%;background:#071007b8;box-shadow:0 0 45px #79dd681c;animation:girarSello 18s linear infinite}.hero-sello span,.hero-sello small{font-size:8px;letter-spacing:.18em;color:#9bad98}.hero-sello strong{font-size:38px;line-height:1;color:#79dd68;margin:5px}
.hero-scroll{position:absolute;z-index:4;left:clamp(25px,7vw,110px);bottom:24px;display:flex;align-items:center;gap:12px;color:#778174;font-size:8px;letter-spacing:.18em}.hero-scroll span{width:75px;height:1px;overflow:hidden;background:#ffffff1f}.hero-scroll span:after{content:"";display:block;width:32px;height:1px;background:#79dd68;animation:recorrer 2.2s ease-in-out infinite}
.card{min-height:370px;background:#161916}.media-card{inset:1px;border-radius:11px}.media-card :deep(img){transform:scale(1.035);filter:saturate(.86) contrast(1.08)}.media-card:before{content:"";position:absolute;inset:0;z-index:2;pointer-events:none;background:repeating-linear-gradient(0deg,#ffffff05 0 1px,transparent 1px 4px),radial-gradient(circle at 50% 38%,transparent 0 25%,#05070545 78%)}
.card:hover .media-card :deep(img){transform:scale(1.07)}.info{padding:22px;background:linear-gradient(0deg,#090b09 20%,transparent)}
@keyframes cascadaSteam{to{transform:translateX(calc(-100% / 3))}}@keyframes cascadaSteamInversa{from{transform:translateX(calc(-100% / 3))}to{transform:translateX(0)}}@keyframes girarSello{to{transform:rotate(360deg)}}@keyframes recorrer{0%{transform:translateX(-35px)}100%{transform:translateX(80px)}}
@media(max-width:850px){.hero-estatico{min-height:620px}.hero-estatico h1{font-size:clamp(68px,15vw,108px)}.muro-productos{opacity:.54;inset:-12% -40%}.hero-sello{right:24px;bottom:35px;width:95px;height:95px}.hero-sello strong{font-size:28px}}
@media(max-width:560px){.hero-estatico{min-height:590px;padding:70px 22px 90px}.hero-estatico h1{font-size:clamp(58px,18vw,84px)}.hero-estatico .hero-copy>p:not(.tag){font-size:15px}.hero-estatico .hero-acciones{flex-direction:column;align-items:stretch}.hero-estatico .hero-acciones a,.hero-estatico .hero-acciones button{justify-content:center}.hero-sello{display:none}.muro-productos{inset:-4% -90%;opacity:.48}.hero-scroll{left:22px}}
@media(prefers-reduced-motion:reduce){.muro-productos{transition:none}.cinta-caratulas,.hero-sello,.hero-scroll span:after{animation:none}}
.grid{grid-template-columns:repeat(4,minmax(0,1fr));gap:18px}.card{min-height:340px}
@media(max-width:1100px){.grid{grid-template-columns:repeat(3,minmax(0,1fr))}}
@media(max-width:850px){.grid{grid-template-columns:repeat(2,minmax(0,1fr))}}
@media(max-width:560px){.grid{grid-template-columns:1fr}}

/* Fondo ambiental del catálogo: solo usa portadas de videojuegos registradas. */
.fondo-videojuegos{position:fixed;inset:82px 0 0;z-index:0;display:grid;grid-template-columns:repeat(5,minmax(0,1fr));gap:18px;padding:0 2vw;overflow:hidden;pointer-events:none;opacity:.34;filter:blur(1.1px) saturate(.9) contrast(1.08);mask-image:linear-gradient(180deg,transparent 0,#000 8%,#000 92%,transparent)}
.fondo-videojuegos:after{content:"";position:absolute;inset:0;background:radial-gradient(circle at 50% 35%,transparent,#080a0870 78%),linear-gradient(90deg,#0b0d0b8c,transparent 22% 78%,#0b0d0b8c)}
.columna-videojuegos{min-width:0;overflow:hidden}.cinta-vertical{display:flex;flex-direction:column;width:100%;animation:caidaVideojuegos 78s linear infinite;will-change:transform}.columna-videojuegos:nth-child(even) .cinta-vertical{animation-name:subidaVideojuegos;animation-duration:88s}.columna-3 .cinta-vertical{animation-duration:96s}.columna-5 .cinta-vertical{animation-duration:82s}
.grupo-vertical{display:flex;flex-direction:column;gap:18px;padding-bottom:18px}.portada-fondo{width:100%;aspect-ratio:4/3;overflow:hidden;border:1px solid #79dd6820;border-radius:9px;background:#111}.portada-fondo :deep(.visual){min-height:0}.portada-fondo :deep(.visual small){display:none}.portada-fondo :deep(img){transform:scale(1.04)}
.hero-estatico,.catalogo{position:relative;z-index:1}.catalogo{background:linear-gradient(180deg,#101010ad,#101010c4 35%,#101010b8)}
@keyframes caidaVideojuegos{from{transform:translateY(calc(-100% / 3))}to{transform:translateY(0)}}@keyframes subidaVideojuegos{from{transform:translateY(0)}to{transform:translateY(calc(-100% / 3))}}
@media(max-width:850px){.fondo-videojuegos{grid-template-columns:repeat(3,minmax(0,1fr));opacity:.25}.columna-4,.columna-5{display:none}}
@media(max-width:560px){.fondo-videojuegos{grid-template-columns:repeat(2,minmax(0,1fr));opacity:.2}.columna-3{display:none}}
@media(prefers-reduced-motion:reduce){.cinta-vertical{animation:none}}
</style>
