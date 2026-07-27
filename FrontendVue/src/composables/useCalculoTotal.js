// Patrón Strategy: distintas reglas de cálculo de total intercambiables
// (venta normal vs. venta con descuento), seleccionables en tiempo de
// ejecución sin tocar el código que las consume (el store del carrito).
const estrategiasTotal = {
  normal: (subtotal) => subtotal,
  descuento10: (subtotal) => subtotal * 0.9,
};

export function calcularTotal(subtotal, estrategia = 'normal') {
  const aplicarEstrategia = estrategiasTotal[estrategia] ?? estrategiasTotal.normal;
  return aplicarEstrategia(subtotal);
}
