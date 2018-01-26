
function MeasureModel(id, label, subMeasure) {
    var self = this;
    this.id = id || 0;
    this.label = label || "";
    this.subMeasure = subMeasure || false;
}
function CoefficientModel(id, scoreId, wave, measureId, coefficient) {
    this.ID = id || 0;
    this.ScoreId = scoreId || 0;
    this.Wave = wave || 1;
    this.Measure = measureId || 0;
    this.Coefficient = coefficient || 0;
}
function BandModel(id, scoreId, wave, ageMin, ageMax, ageOrWaveMean, ageOrWave) {
    this.ID = id || 0;
    this.ScoreId = scoreId || 0;
    this.Wave = wave || 1;
    this.AgeMin = ageMin || 0;
    this.AgeMax = ageMax || 0;
    this.AgeOrWaveMean = ageOrWaveMean || 0;
    this.AgeOrWave = ageOrWave || 0;
}
function CustomScoreModel(measures, coefficients, bands) {
    var self = this;
    this.measures = [];
    this.setSubMeasure = function (option, item) {
        if (item.subMeasure) {
            ko.applyBindingsToNode(option, { html: '&nbsp;&nbsp;&nbsp;&nbsp;' + item.label }, item);
        }
    }
    /*------------------------------coefficients start-------------------------------------*/
    this.coefficients = ko.observableArray();
    this.addCoefficient = function (wave) {
        var newCoefficient = new CoefficientModel();
        newCoefficient.Wave = wave;
        self.coefficients.push(newCoefficient);
    };
    this.delCoefficient = function (coefficient) {
        self.coefficients.remove(coefficient);
        self.ensureCoefficientofWave(coefficient.Wave);
    };
    this.ensureCoefficientofWave = function (wave) {

        var exists = false;
        $.each(self.coefficients(), function (index, coefficient) {
            if (coefficient.Wave == wave) {
                exists = true;
                return false;
            }
        });
        if (exists == false) {
            self.addCoefficient(wave);
        }
    }
    /*------------------------------coefficients end--------------------------------------*/

    /*------------------------------bands start-------------------------------------------*/
    this.bands = ko.observableArray();
    this.addBand = function (wave) {
        var newBand = new BandModel();
        newBand.Wave = wave;
        self.bands.push(newBand);
    };
    this.delBand = function (band) {
        self.bands.remove(band);
        self.ensureBandWave(band.Wave);
    };
    this.ensureBandWave = function (wave) {
        var exists = false;
        $.each(self.bands(), function (index, band) {
            if (band.Wave == wave) {
                exists = true;
                return false;
            }
        });
        if (exists == false) {
            self.addBand(wave);
        }
    }
    /*------------------------------bands end--------------------------------------------*/
    this.init = function () {
        self.measures.push(new MeasureModel(0, 'Please Select...'));
        if (measures) {
            $.each(measures, function (i, e) {
                var subMeasures = [];
                if (e.ParentId > 1) {
                    return;
                }
                else {
                    $.each(measures, function (j, sub) {
                        if (sub.ParentId == e.ID) {
                            subMeasures.push({ ID: sub.ID, Label: sub.Label });
                        }
                    });
                }
                self.measures.push(new MeasureModel(e.ID, e.Label));
                if (subMeasures.length) {
                    $.each(subMeasures, function (j, sub) {
                        self.measures.push(new MeasureModel(sub.ID, '   ' + sub.Label, true));
                    })
                }
            });
        }
        if (coefficients && coefficients.length) {
            $.each(coefficients, function (i, e) {
                self.coefficients.push(new CoefficientModel(e.ID, e.ScoreId, e.Wave.value, e.Measure, e.Coefficient));
            });
        }
        self.ensureCoefficientofWave(1);
        self.ensureCoefficientofWave(2);
        self.ensureCoefficientofWave(3);

        if (bands && bands.length) {
            $.each(bands, function (i, e) {
                self.bands.push(new BandModel(e.ID, e.ScoreId, e.Wave.value, e.AgeMin, e.AgeMax, e.AgeOrWaveMean, e.AgeOrWave));
            });
        }
        self.ensureBandWave(1);
        self.ensureBandWave(2);
        self.ensureBandWave(3);
    };
    this.init();
}