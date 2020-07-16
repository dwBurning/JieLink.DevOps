package com.jieshun.devopsserver.bean;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class VersionInfoExample {
    protected String orderByClause;

    protected boolean distinct;

    protected List<Criteria> oredCriteria;

    public VersionInfoExample() {
        oredCriteria = new ArrayList<Criteria>();
    }

    public void setOrderByClause(String orderByClause) {
        this.orderByClause = orderByClause;
    }

    public String getOrderByClause() {
        return orderByClause;
    }

    public void setDistinct(boolean distinct) {
        this.distinct = distinct;
    }

    public boolean isDistinct() {
        return distinct;
    }

    public List<Criteria> getOredCriteria() {
        return oredCriteria;
    }

    public void or(Criteria criteria) {
        oredCriteria.add(criteria);
    }

    public Criteria or() {
        Criteria criteria = createCriteriaInternal();
        oredCriteria.add(criteria);
        return criteria;
    }

    public Criteria createCriteria() {
        Criteria criteria = createCriteriaInternal();
        if (oredCriteria.size() == 0) {
            oredCriteria.add(criteria);
        }
        return criteria;
    }

    protected Criteria createCriteriaInternal() {
        Criteria criteria = new Criteria();
        return criteria;
    }

    public void clear() {
        oredCriteria.clear();
        orderByClause = null;
        distinct = false;
    }

    protected abstract static class GeneratedCriteria {
        protected List<Criterion> criteria;

        protected GeneratedCriteria() {
            super();
            criteria = new ArrayList<Criterion>();
        }

        public boolean isValid() {
            return criteria.size() > 0;
        }

        public List<Criterion> getAllCriteria() {
            return criteria;
        }

        public List<Criterion> getCriteria() {
            return criteria;
        }

        protected void addCriterion(String condition) {
            if (condition == null) {
                throw new RuntimeException("Value for condition cannot be null");
            }
            criteria.add(new Criterion(condition));
        }

        protected void addCriterion(String condition, Object value, String property) {
            if (value == null) {
                throw new RuntimeException("Value for " + property + " cannot be null");
            }
            criteria.add(new Criterion(condition, value));
        }

        protected void addCriterion(String condition, Object value1, Object value2, String property) {
            if (value1 == null || value2 == null) {
                throw new RuntimeException("Between values for " + property + " cannot be null");
            }
            criteria.add(new Criterion(condition, value1, value2));
        }

        public Criteria andIdIsNull() {
            addCriterion("id is null");
            return (Criteria) this;
        }

        public Criteria andIdIsNotNull() {
            addCriterion("id is not null");
            return (Criteria) this;
        }

        public Criteria andIdEqualTo(Integer value) {
            addCriterion("id =", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdNotEqualTo(Integer value) {
            addCriterion("id <>", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdGreaterThan(Integer value) {
            addCriterion("id >", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdGreaterThanOrEqualTo(Integer value) {
            addCriterion("id >=", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdLessThan(Integer value) {
            addCriterion("id <", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdLessThanOrEqualTo(Integer value) {
            addCriterion("id <=", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdIn(List<Integer> values) {
            addCriterion("id in", values, "id");
            return (Criteria) this;
        }

        public Criteria andIdNotIn(List<Integer> values) {
            addCriterion("id not in", values, "id");
            return (Criteria) this;
        }

        public Criteria andIdBetween(Integer value1, Integer value2) {
            addCriterion("id between", value1, value2, "id");
            return (Criteria) this;
        }

        public Criteria andIdNotBetween(Integer value1, Integer value2) {
            addCriterion("id not between", value1, value2, "id");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoIsNull() {
            addCriterion("work_order_no is null");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoIsNotNull() {
            addCriterion("work_order_no is not null");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoEqualTo(String value) {
            addCriterion("work_order_no =", value, "workOrderNo");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoNotEqualTo(String value) {
            addCriterion("work_order_no <>", value, "workOrderNo");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoGreaterThan(String value) {
            addCriterion("work_order_no >", value, "workOrderNo");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoGreaterThanOrEqualTo(String value) {
            addCriterion("work_order_no >=", value, "workOrderNo");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoLessThan(String value) {
            addCriterion("work_order_no <", value, "workOrderNo");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoLessThanOrEqualTo(String value) {
            addCriterion("work_order_no <=", value, "workOrderNo");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoLike(String value) {
            addCriterion("work_order_no like", value, "workOrderNo");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoNotLike(String value) {
            addCriterion("work_order_no not like", value, "workOrderNo");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoIn(List<String> values) {
            addCriterion("work_order_no in", values, "workOrderNo");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoNotIn(List<String> values) {
            addCriterion("work_order_no not in", values, "workOrderNo");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoBetween(String value1, String value2) {
            addCriterion("work_order_no between", value1, value2, "workOrderNo");
            return (Criteria) this;
        }

        public Criteria andWorkOrderNoNotBetween(String value1, String value2) {
            addCriterion("work_order_no not between", value1, value2, "workOrderNo");
            return (Criteria) this;
        }

        public Criteria andStandVersionIsNull() {
            addCriterion("stand_version is null");
            return (Criteria) this;
        }

        public Criteria andStandVersionIsNotNull() {
            addCriterion("stand_version is not null");
            return (Criteria) this;
        }

        public Criteria andStandVersionEqualTo(String value) {
            addCriterion("stand_version =", value, "standVersion");
            return (Criteria) this;
        }

        public Criteria andStandVersionNotEqualTo(String value) {
            addCriterion("stand_version <>", value, "standVersion");
            return (Criteria) this;
        }

        public Criteria andStandVersionGreaterThan(String value) {
            addCriterion("stand_version >", value, "standVersion");
            return (Criteria) this;
        }

        public Criteria andStandVersionGreaterThanOrEqualTo(String value) {
            addCriterion("stand_version >=", value, "standVersion");
            return (Criteria) this;
        }

        public Criteria andStandVersionLessThan(String value) {
            addCriterion("stand_version <", value, "standVersion");
            return (Criteria) this;
        }

        public Criteria andStandVersionLessThanOrEqualTo(String value) {
            addCriterion("stand_version <=", value, "standVersion");
            return (Criteria) this;
        }

        public Criteria andStandVersionLike(String value) {
            addCriterion("stand_version like", value, "standVersion");
            return (Criteria) this;
        }

        public Criteria andStandVersionNotLike(String value) {
            addCriterion("stand_version not like", value, "standVersion");
            return (Criteria) this;
        }

        public Criteria andStandVersionIn(List<String> values) {
            addCriterion("stand_version in", values, "standVersion");
            return (Criteria) this;
        }

        public Criteria andStandVersionNotIn(List<String> values) {
            addCriterion("stand_version not in", values, "standVersion");
            return (Criteria) this;
        }

        public Criteria andStandVersionBetween(String value1, String value2) {
            addCriterion("stand_version between", value1, value2, "standVersion");
            return (Criteria) this;
        }

        public Criteria andStandVersionNotBetween(String value1, String value2) {
            addCriterion("stand_version not between", value1, value2, "standVersion");
            return (Criteria) this;
        }

        public Criteria andVersionTypeIsNull() {
            addCriterion("version_type is null");
            return (Criteria) this;
        }

        public Criteria andVersionTypeIsNotNull() {
            addCriterion("version_type is not null");
            return (Criteria) this;
        }

        public Criteria andVersionTypeEqualTo(Integer value) {
            addCriterion("version_type =", value, "versionType");
            return (Criteria) this;
        }

        public Criteria andVersionTypeNotEqualTo(Integer value) {
            addCriterion("version_type <>", value, "versionType");
            return (Criteria) this;
        }

        public Criteria andVersionTypeGreaterThan(Integer value) {
            addCriterion("version_type >", value, "versionType");
            return (Criteria) this;
        }

        public Criteria andVersionTypeGreaterThanOrEqualTo(Integer value) {
            addCriterion("version_type >=", value, "versionType");
            return (Criteria) this;
        }

        public Criteria andVersionTypeLessThan(Integer value) {
            addCriterion("version_type <", value, "versionType");
            return (Criteria) this;
        }

        public Criteria andVersionTypeLessThanOrEqualTo(Integer value) {
            addCriterion("version_type <=", value, "versionType");
            return (Criteria) this;
        }

        public Criteria andVersionTypeIn(List<Integer> values) {
            addCriterion("version_type in", values, "versionType");
            return (Criteria) this;
        }

        public Criteria andVersionTypeNotIn(List<Integer> values) {
            addCriterion("version_type not in", values, "versionType");
            return (Criteria) this;
        }

        public Criteria andVersionTypeBetween(Integer value1, Integer value2) {
            addCriterion("version_type between", value1, value2, "versionType");
            return (Criteria) this;
        }

        public Criteria andVersionTypeNotBetween(Integer value1, Integer value2) {
            addCriterion("version_type not between", value1, value2, "versionType");
            return (Criteria) this;
        }

        public Criteria andCompileDateIsNull() {
            addCriterion("compile_date is null");
            return (Criteria) this;
        }

        public Criteria andCompileDateIsNotNull() {
            addCriterion("compile_date is not null");
            return (Criteria) this;
        }

        public Criteria andCompileDateEqualTo(Date value) {
            addCriterion("compile_date =", value, "compileDate");
            return (Criteria) this;
        }

        public Criteria andCompileDateNotEqualTo(Date value) {
            addCriterion("compile_date <>", value, "compileDate");
            return (Criteria) this;
        }

        public Criteria andCompileDateGreaterThan(Date value) {
            addCriterion("compile_date >", value, "compileDate");
            return (Criteria) this;
        }

        public Criteria andCompileDateGreaterThanOrEqualTo(Date value) {
            addCriterion("compile_date >=", value, "compileDate");
            return (Criteria) this;
        }

        public Criteria andCompileDateLessThan(Date value) {
            addCriterion("compile_date <", value, "compileDate");
            return (Criteria) this;
        }

        public Criteria andCompileDateLessThanOrEqualTo(Date value) {
            addCriterion("compile_date <=", value, "compileDate");
            return (Criteria) this;
        }

        public Criteria andCompileDateIn(List<Date> values) {
            addCriterion("compile_date in", values, "compileDate");
            return (Criteria) this;
        }

        public Criteria andCompileDateNotIn(List<Date> values) {
            addCriterion("compile_date not in", values, "compileDate");
            return (Criteria) this;
        }

        public Criteria andCompileDateBetween(Date value1, Date value2) {
            addCriterion("compile_date between", value1, value2, "compileDate");
            return (Criteria) this;
        }

        public Criteria andCompileDateNotBetween(Date value1, Date value2) {
            addCriterion("compile_date not between", value1, value2, "compileDate");
            return (Criteria) this;
        }
    }

    public static class Criteria extends GeneratedCriteria {

        protected Criteria() {
            super();
        }
    }

    public static class Criterion {
        private String condition;

        private Object value;

        private Object secondValue;

        private boolean noValue;

        private boolean singleValue;

        private boolean betweenValue;

        private boolean listValue;

        private String typeHandler;

        public String getCondition() {
            return condition;
        }

        public Object getValue() {
            return value;
        }

        public Object getSecondValue() {
            return secondValue;
        }

        public boolean isNoValue() {
            return noValue;
        }

        public boolean isSingleValue() {
            return singleValue;
        }

        public boolean isBetweenValue() {
            return betweenValue;
        }

        public boolean isListValue() {
            return listValue;
        }

        public String getTypeHandler() {
            return typeHandler;
        }

        protected Criterion(String condition) {
            super();
            this.condition = condition;
            this.typeHandler = null;
            this.noValue = true;
        }

        protected Criterion(String condition, Object value, String typeHandler) {
            super();
            this.condition = condition;
            this.value = value;
            this.typeHandler = typeHandler;
            if (value instanceof List<?>) {
                this.listValue = true;
            } else {
                this.singleValue = true;
            }
        }

        protected Criterion(String condition, Object value) {
            this(condition, value, null);
        }

        protected Criterion(String condition, Object value, Object secondValue, String typeHandler) {
            super();
            this.condition = condition;
            this.value = value;
            this.secondValue = secondValue;
            this.typeHandler = typeHandler;
            this.betweenValue = true;
        }

        protected Criterion(String condition, Object value, Object secondValue) {
            this(condition, value, secondValue, null);
        }
    }
}