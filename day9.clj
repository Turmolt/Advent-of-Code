(ns adventofcode.day9
  (:require [adventofcode.util :as u]
            [adventofcode.day5 :as cpu]))

(defn solve [i]
  (->> (u/input-csv 9)
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