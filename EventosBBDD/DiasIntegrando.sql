DELIMITER //

CREATE EVENT IF NOT EXISTS actualizar_dias_integrando
ON SCHEDULE EVERY 1 DAY
STARTS CURRENT_DATE + INTERVAL 1 DAY
DO
BEGIN
  DECLARE done INT DEFAULT 0;
  DECLARE v_id INT;
  DECLARE v_ultimo_dia DATE;
  DECLARE cur CURSOR FOR
    SELECT Id, UltimoDiaIntegrando
    FROM integraciones
    WHERE StandBy = FALSE AND Certificado = FALSE;

  DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;

  OPEN cur;

  read_loop: LOOP
    FETCH cur INTO v_id, v_ultimo_dia;
    IF done THEN
      LEAVE read_loop;
    END IF;

    IF v_ultimo_dia IS NOT NULL THEN
      SET @row := 0;

      UPDATE integraciones
      SET DiasIntegrando = DiasIntegrando + (
        SELECT COUNT(*) FROM (
          SELECT CURRENT_DATE - INTERVAL seq DAY AS fecha
          FROM (
            SELECT @row := @row + 1 AS seq
            FROM (SELECT 0 UNION SELECT 1 UNION SELECT 2 UNION SELECT 3 UNION SELECT 4
                  UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 UNION SELECT 9) AS t1,
                 (SELECT 0 UNION SELECT 1 UNION SELECT 2 UNION SELECT 3 UNION SELECT 4
                  UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 UNION SELECT 9) AS t2
          ) AS fechas
          WHERE fecha > v_ultimo_dia AND fecha < CURRENT_DATE
          AND DAYOFWEEK(fecha) BETWEEN 2 AND 6
        ) AS dias_laborales
      ),
      UltimoDiaIntegrando = CURRENT_DATE
      WHERE Id = v_id;
    END IF;
  END LOOP;

  CLOSE cur;
END;
//

DELIMITER ;
