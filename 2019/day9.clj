(ns adventofcode.2019.day9
  (:require [adventofcode.util :as u]
            [adventofcode.2019.day5 :as cpu]))

(defn solve [i]
  (->> (u/input-csv 2019 9)
       (cpu/create-memory)
       (#(cpu/solve % i 0))))

(defn part-one [] (solve 1))

(defn part-two [] (solve 2))

(time (part-one))
;; => 3454977457
;; => "Elapsed time: 3.3713 msecs"

(time (part-two))
;; => 50120
;; => "Elapsed time: 5589.48 msecs"