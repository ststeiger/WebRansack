"use strict";

function autoBind(self)
{
    for (var _i = 0, _a = Object.getOwnPropertyNames(self.constructor.prototype); _i < _a.length; _i++)
    {
        var key = _a[_i];
        if (key === 'constructor')
            continue;
        var desc = Object.getOwnPropertyDescriptor(self.constructor.prototype, key);
        if (desc == null)
            continue;
        if (!desc.configurable)
        {
            console.log("AUTOBIND-WARNING: Property \"" + key + "\" not configurable ! (" + self.constructor.name + ")");
            continue;
        }
        var g = desc.get != null;
        var s = desc.set != null;
        if (g || s)
        {
            var newDescriptor = {};
            newDescriptor.enumerable = desc.enumerable;
            newDescriptor.configurable = desc.configurable;
            if (g)
                newDescriptor.get = desc.get.bind(self);
            if (s)
                newDescriptor.set = desc.set.bind(self);
            Object.defineProperty(self, key, newDescriptor);
            continue;
        }
        if (typeof (desc.value) === 'function')
        {
            self[key] = desc.value.bind(self);
        }
    }
    return self;
}

function autoTrace(self)
{
    function getLoggableFunction_old(func, type, name)
    {
        return function ()
        {
            var args = [];
            for (var _i = 0; _i < arguments.length; _i++)
            {
                args[_i] = arguments[_i];
            }
            var logText = name + '(';
            for (var i = 0; i < args.length; i++)
            {
                if (i > 0)
                {
                    logText += ', ';
                }
                logText += args[i];
            }
            logText += ');';
            console.log(type + " " + logText);
            return func.apply(self, args);
        };
    }
    function getLoggableFunction(func, type, name)
    {
        return function ()
        {
            var args = [];
            for (var _i = 0; _i < arguments.length; _i++)
            {
                args[_i] = arguments[_i];
            }
            var logText = name + '(';
            for (var i = 0; i < args.length; i++)
            {
                if (i > 0)
                {
                    logText += ', ';
                }
                logText += args[i];
            }
            logText += ')';
            console.log("Pre " + type + " " + logText + "; ");
            var res = func.apply(self, args);
            console.log("Post " + type + " " + logText + ":", res);
            return res;
        };
    }
    for (var _i = 0, _a = Object.getOwnPropertyNames(self.constructor.prototype); _i < _a.length; _i++)
    {
        var key = _a[_i];
        if (key === 'constructor')
            continue;
        var desc = Object.getOwnPropertyDescriptor(self.constructor.prototype, key);
        if (desc == null)
            continue;
        if (!desc.configurable)
        {
            console.log("AUTOTRACE-WARNING: Property \"" + key + "\" not configurable ! (" + self.constructor.name + ")");
            continue;
        }
        var g = desc.get != null;
        var s = desc.set != null;
        if (g || s)
        {
            var newDescriptor = {};
            newDescriptor.enumerable = desc.enumerable;
            newDescriptor.configurable = desc.configurable;
            if (g)
                newDescriptor.get = getLoggableFunction(desc.get.bind(self), "Property", "get_" + key);
            if (s)
                newDescriptor.set = getLoggableFunction(desc.set.bind(self), "Property", "set_" + key);
            Object.defineProperty(self, key, newDescriptor);
            continue;
        }
        if (typeof (desc.value) === 'function')
        {
            self[key] = getLoggableFunction(desc.value.bind(self), "Function", key);
        }
    }
    return self;
}

function groupBy(source, keySelector)
{
    if (source == null)
        throw new Error("ArgumentNullException: Source");
    if (keySelector == null)
        throw new Error("ArgumentNullException: keySelector");
    var dict = {};
    for (var i = 0; i < source.length; ++i)
    {
        var key = String(keySelector(source[i])) || "null";
        if (!dict.hasOwnProperty(key))
        {
            dict[key] = [];
        }
        dict[key].push(source[i]);
    }
    return dict;
}


var GroupedData = (function ()
{
    function GroupedData(key, columns, columnMap, data)
    {
        (0, autoBind)(this);
        var that = this;
        this.m_accessor = {};
        this.m_key = key;
        this.m_columns = columns;
        this.m_columnMap = columnMap;
        this.m_groupedData = data || null;
        var _loop_1 = function (i)
        {
            var propName = that.columns[i];
            Object.defineProperty(that.m_accessor, propName, {
                get: function ()
                {
                    if (that.m_groupedData == null)
                        return null;
                    var currentRow = that.m_groupedData[that.m_i];
                    return currentRow == null ? currentRow : currentRow[i];
                }.bind(that),
                set: function (value)
                {
                    if (that.m_groupedData == null)
                        that.m_groupedData = [];
                    var currentRow = that.m_groupedData[that.m_i];
                    if (currentRow != null)
                        currentRow[i] = value;
                }.bind(that),
                enumerable: true,
                configurable: true
            });
        };
        for (var i = 0; i < that.columns.length; ++i)
        {
            _loop_1(i);
        }
    }
    Object.defineProperty(GroupedData.prototype, "key", {
        get: function ()
        {
            return this.m_key;
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(GroupedData.prototype, "columns", {
        get: function ()
        {
            return this.m_columns;
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(GroupedData.prototype, "rows", {
        get: function ()
        {
            if (this.m_groupedData == null)
                return [];
            return this.m_groupedData;
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(GroupedData.prototype, "rowCount", {
        get: function ()
        {
            if (this.m_groupedData == null)
                return 0;
            return this.m_groupedData.length;
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(GroupedData.prototype, "columnMap", {
        get: function ()
        {
            return this.m_columnMap;
        },
        enumerable: false,
        configurable: true
    });
    GroupedData.prototype.row = function (i)
    {
        this.m_i = i;
        return this.m_accessor;
    };
    GroupedData.prototype.getIndex = function (name)
    {
        return this.m_columnMap[name];
    };
    return GroupedData;
}());

var GroupedTableWrapper = (function ()
{
    function GroupedTableWrapper(tw, propertyToGroupBy)
    {
        var that = this;
        (0, autoBind)(this);
        this.m_columnMap = tw.columnMap;
        this.m_columns = tw.columns;
        this.m_columnLength = tw.columns.length;
        this.m_groupedData = (0, linq_js_1.groupBy)(tw.rows, function (item)
        {
            return item[tw.getIndex(propertyToGroupBy)];
        });
        this.m_accessor = {};
        var _loop_2 = function (i)
        {
            var propName = this_1.m_columns[i];
            Object.defineProperty(this_1.m_accessor, propName, {
                get: function ()
                {
                    var currentRow = that.m_groupedData[that.m_id];
                    return currentRow == null ? null : currentRow[i];
                }.bind(that),
                set: function (value)
                {
                    if (that.m_groupedData[that.m_id] == null)
                        that.m_groupedData[that.m_id] = [];
                    that.m_groupedData[that.m_id][i] = value;
                }.bind(that),
                enumerable: true,
                configurable: true
            });
        };
        var this_1 = this;
        for (var i = 0; i < this.m_columnLength; ++i)
        {
            _loop_2(i);
        }
    }
    Object.defineProperty(GroupedTableWrapper.prototype, "columnCount", {
        get: function ()
        {
            return this.m_columns.length;
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(GroupedTableWrapper.prototype, "columns", {
        get: function ()
        {
            return this.m_columns;
        },
        enumerable: false,
        configurable: true
    });
    GroupedTableWrapper.prototype.getGroup = function (id)
    {
        return new GroupedData(id, this.columns, this.m_columnMap, this.m_groupedData[id]);
    };
    GroupedTableWrapper.prototype.getIndex = function (name)
    {
        return this.m_columnMap[name];
    };
    return GroupedTableWrapper;
}());

var TableWrapper = (function ()
{
    function TableWrapper(columns, data, ignoreCase)
    {
        var that = this;
        (0, autoBind)(this);
        if (ignoreCase == null)
            ignoreCase = true;
        for (var i = 0; i < columns.length; ++i)
        {
            if (ignoreCase)
                columns[i] = columns[i].toLowerCase();
        }
        this.rows = data;
        this.setColumns(columns);
        this.m_accessor = {};
        var _loop_3 = function (i)
        {
            var propName = columns[i];
            Object.defineProperty(this_2.m_accessor, propName, {
                get: function ()
                {
                    var currentRow = that.rows[that.m_i];
                    return currentRow == null ? null : currentRow[i];
                }.bind(that),
                set: function (value)
                {
                    if (that.rows[that.m_i] == null)
                        that.rows[that.m_i] = [];
                    that.rows[that.m_i][i] = value;
                }.bind(that),
                enumerable: true,
                configurable: true
            });
        };
        var this_2 = this;
        for (var i = 0; i < columns.length; ++i)
        {
            _loop_3(i);
        }
    }
    Object.defineProperty(TableWrapper.prototype, "columnMap", {
        get: function ()
        {
            return this.m_columnMap;
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(TableWrapper.prototype, "rowCount", {
        get: function ()
        {
            return this.rows.length;
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(TableWrapper.prototype, "columnCount", {
        get: function ()
        {
            return this.m_columns.length;
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(TableWrapper.prototype, "columns", {
        get: function ()
        {
            return this.m_columns;
        },
        enumerable: false,
        configurable: true
    });
    TableWrapper.prototype.setColumns = function (cols)
    {
        this.m_columnLength = cols.length;
        this.m_columnMap = null;
        this.m_columnMap = {};
        for (var i = 0; i < this.m_columnLength; ++i)
        {
            this.m_columnMap[cols[i]] = i;
        }
        this.m_columns = cols;
    };
    TableWrapper.prototype.row = function (i)
    {
        this.m_i = i;
        return this.m_accessor;
    };
    TableWrapper.prototype.getIndex = function (name)
    {
        return this.m_columnMap[name];
    };
    TableWrapper.prototype.addRow = function (dat)
    {
        this.rows.push(dat);
        return this;
    };
    TableWrapper.prototype.removeRow = function (i)
    {
        this.rows.splice(i, 1);
        return this;
    };
    TableWrapper.prototype.groupBy = function (key)
    {
        return new GroupedTableWrapper(this, key);
    };
    return TableWrapper;
}());
