(ns adventofcode.2021.day7
  (:require [adventofcode.util :as u]))

(def data (u/input-csv 2021 99))

(def average (int (float (/ (reduce + data) (count data)))))

(reduce + (mapv (fn [v] (Math/abs (- v average))) data))