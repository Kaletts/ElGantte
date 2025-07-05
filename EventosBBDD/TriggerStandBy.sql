DELIMITER //

CREATE TRIGGER actualizar_ultimos_dias
AFTER UPDATE ON integraciones
FOR EACH ROW
BEGIN
  -- Cuando cambia a StandBy = TRUE ESTO NO FUNCIONA - NO USAR, MYSQL NO DEJA QUE UN TRIGGER ACTUE SOBRE LA MISMA TABLA QUE ESTAS MODIFICANDO
  IF NEW.StandBy = TRUE AND (OLD.StandBy IS NULL OR OLD.StandBy = FALSE) THEN
    UPDATE integraciones
    SET UltimoDiaStandBy = CURRENT_DATE
    WHERE Id = NEW.Id;
  END IF;

  -- Cuando cambia a StandBy = FALSE
  IF NEW.StandBy = FALSE AND (OLD.StandBy IS NULL OR OLD.StandBy = TRUE) THEN
    UPDATE integraciones
    SET UltimoDiaIntegrando = CURRENT_DATE
    WHERE Id = NEW.Id;
  END IF;
END;
//

DELIMITER ;
