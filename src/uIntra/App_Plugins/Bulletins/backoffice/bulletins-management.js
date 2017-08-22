﻿(function (angular) {
    'use strict';

    var onError = function (error) { console.error(error); }

    var controller = function ($http, authResource, $scope, bulletinsManagementConfig, bulletinsManagementFactory) {
        var self = this;
        self.bulletinsList = [];
        self.currentUser = null;
        self.dateFormat = "dd/MM/yyyy";
        self.selected = null;
        self.selectedIndex = null;
        self.filterModel = {};

        self.filter = function (item) {
            var checkList = [];

            var compareText = function (left, right) {
                return left.toLowerCase().indexOf(right.toLowerCase()) > -1;
            }

            var compareDates = function (itemDate, filter) {
                var date = new Date(itemDate);
                date.setHours(0, 0, 0, 0);

                if (filter) {
                    if (filter.from) {
                        checkList.push(date >= new Date(filter.from));
                    }
                    if (filter.to) {
                        checkList.push(date <= new Date(filter.to));
                    }
                }
            }

            if (self.filterModel.id) {
                checkList.push(compareText(item.id, self.filterModel.id));
            }
            if (self.filterModel.description) {
                checkList.push(compareText(item.description, self.filterModel.description));
            }
            if (self.filterModel.publishDate) {
                compareDates(item.publishDate, self.filterModel.publishDate);
            }
            if (self.filterModel.createdDate) {
                compareDates(item.createdDate, self.filterModel.createdDate);
            }
            if (self.filterModel.modifyDate) {
                compareDates(item.modifyDate, self.filterModel.modifyDate);
            }


            return checkList.filter(function (item) { return !!item; }).length == checkList.length;
        }

        self.getHeadClasses = function () {
            return {
                '_admin-mode': self.currentUser != null && self.currentUser.id == 0,
                '_bulletins-selected': self.selected != null
            };
        }

        self.selectBulletinsToEdit = function (bulletin, index) {
            self.selectedIndex = index;
            self.selected = angular.copy(bulletin);
            self.selected.publishDate = self.selected.publishDate || new Date().toISOString();
            self.selected.umbracoCreatorId = self.selected.umbracoCreatorId || self.currentUser.id;
        }

        self.save = function () {
            if ($scope.editForm.$invalid) {
                $scope.editForm.$setDirty();
                return;
            }

            if (self.selected.id == null) {
                create(self.selected);
            } else {
                save(self.selected, self.selectedIndex);
            }
        }

        self.delete = function (bulletins, $index) {
            self.clearSelected();
            if (!confirm('Are you sure?')) {
                return;
            }
            
            bulletinsManagementFactory.delete(bulletins.id).then(function (response) {
                self.bulletinsList.splice($index, 1);
                self.clearSelected();
            }, onError);
        }

        self.clearSelected = function () {
            self.selectedIndex = self.selected = null;
        }

        var create = function (bulletin) {
            bulletinsManagementFactory.create(bulletin).then(function (response) {
                self.bulletinsList.push(response.data);
                self.clearSelected();
            }, onError);
        }

        var save = function (bulletins, index) {
            bulletinsManagementFactory.save(bulletins).then(function (response) {
                self.bulletinsList.splice(index, 1, response.data);
                self.clearSelected();
            }, onError);
        }

        var loadAll = function () {
            var success = function (response) {
                self.bulletinsList = response.data.sort(function (prev, next) {
                    return prev.createdDate < next.createdDate ? 1 : -1;
                }) || [];
            }
            bulletinsManagementFactory.getAll().then(success, onError);
        }

        var activate = function () {
            loadAll();
            authResource.getCurrentUser().then(function (data) {
                self.currentUser = data;
            });
            self.config = bulletinsManagementConfig;
        }

        activate();
    }

    controller.$inject = ["$http", "authResource", "$scope", "bulletinsManagementConfig", "bulletinsManagementFactory"];
    angular.module('umbraco').controller('BulletinsManagementController', controller);
})(angular);