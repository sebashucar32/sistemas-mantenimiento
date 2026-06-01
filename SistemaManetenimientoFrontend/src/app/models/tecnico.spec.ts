import {
  MENSAJE_TELEFONO_INVALIDO,
  telefonoEsValido,
  TELEFONO_REGEX,
} from './tecnico';

describe('tecnico model', () => {
  it('TELEFONO_REGEX debería estar definido', () => {
    expect(TELEFONO_REGEX).toBeTruthy();
  });

  it('telefonoEsValido debería validar teléfonos correctos', () => {
    expect(telefonoEsValido('3001234567')).toBeTrue();
    expect(telefonoEsValido('+573001234567')).toBeTrue();
    expect(telefonoEsValido(' 3001234567 ')).toBeTrue();
  });

  it('telefonoEsValido debería rechazar teléfonos inválidos', () => {
    expect(telefonoEsValido('123')).toBeFalse();
    expect(telefonoEsValido('abc')).toBeFalse();
    expect(telefonoEsValido('')).toBeFalse();
  });

  it('MENSAJE_TELEFONO_INVALIDO debería tener texto descriptivo', () => {
    expect(MENSAJE_TELEFONO_INVALIDO).toContain('7 y 15 dígitos');
  });
});
